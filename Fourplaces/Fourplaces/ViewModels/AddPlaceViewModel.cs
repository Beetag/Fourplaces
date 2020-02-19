using Common.Api.Dtos;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TD.Api.Dtos;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class AddPlaceViewModel : ViewModelBase
    {
        private string _title;
        private string _description;
        private string _latitude;
        private string _longitude;
        private string accessToken;
        private string _imagePath;
        private int _image_id;

        public Command AddPlace { get; }
        public Command UseMyCoordinates { get; }
        public Command TakePhoto { get; }
        public Command ChoosePicture { get; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public string Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (SetProperty(ref _imagePath, value) && value != null) { }
            }
        }

        public int ImageId
        {
            get => _image_id;
            set => SetProperty(ref _image_id, value);
        }

        public AddPlaceViewModel()
        {
            accessToken = App.Current.Properties["AccessToken"].ToString();

            AddPlace = new Command(AddNewPlace);
            UseMyCoordinates = new Command(GetMyCoordinates);
            TakePhoto = new Command(OpenCamera);
            ChoosePicture = new Command(LoadPictureFromGallery);
        }

        public async void AddNewPlace()
        {
            if (Title == " " || Title == null)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez saisir un nom de lieu", "Ok");
            }
            else if (Description == " " || Description == null)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez saisir une description", "Ok");
            }
            else if ((Latitude == " " || Latitude == null) || (Longitude == " " || Longitude == null))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez saisir une latitude et une longitude", "Ok");
            }
            else if (ImagePath == " " || ImagePath == null)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez choisir une image pour le lieu", "Ok");
            }
            else
            {
                try
                {
                    if (_imagePath != null && _imagePath != " ")
                    {
                        ImageId = await SendNewImage();
                    }
                    ApiClient apiClient = new ApiClient();
                    int idImage = await SendNewImage();
                    HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/places", new CreatePlaceRequest()
                    {
                        Title = _title,
                        Description = _description,
                        ImageId = idImage,
                        Latitude = double.Parse(_latitude),
                        Longitude = double.Parse(_longitude)
                    }, accessToken);
                    Response<CreatePlaceRequest> result = await apiClient.ReadFromResponse<Response<CreatePlaceRequest>>(response);

                    if (result.IsSuccess)
                    {
                        await Application.Current.MainPage.DisplayAlert("Succès", "Nouveau lieu ajouté", "Ok");
                        await DependencyService.Get<INavigationService>().PopAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Erreur", result.ErrorMessage, "Ok");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            
        }

        private async void GetMyCoordinates(object obj)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium);
                Xamarin.Essentials.Location location = await Geolocation.GetLocationAsync(request);
                Latitude = location.Latitude.ToString();
                Longitude = location.Longitude.ToString();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Debug.WriteLine(fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Debug.WriteLine(fneEx.Message);
            }
            catch (PermissionException pEx)
            {
                Debug.WriteLine(pEx.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void LoadPictureFromGallery(object obj)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur accès", "Impossible d'accéder à la caméra", "Ok");
                return;
            }

            var photo = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = PhotoSize.Small
            });
            if (photo == null)
                return;

            await Application.Current.MainPage.DisplayAlert("Image choisie", photo.Path, "Ok");

            ImagePath = photo.Path;
        }

        private async void OpenCamera(object obj)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur accès", "Impossible d'accéder à la caméra", "Ok");
                return;
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
                PhotoSize = PhotoSize.Small
            });

            if (photo == null)
                return;

            await Application.Current.MainPage.DisplayAlert("Image choisie", photo.Path, "Ok");

            ImagePath = photo.Path;
        }

        private async Task<int> SendNewImage()
        {
            ApiClient apiClient = new ApiClient();
            HttpClient client = new HttpClient();
            byte[] imageData = FromImageToBinary(ImagePath);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://td-api.julienmialon.com/images");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            // Le deuxième paramètre doit absolument être "file" ici sinon ça ne fonctionnera pas
            requestContent.Add(imageContent, "file", "file.jpg");

            request.Content = requestContent;

            HttpResponseMessage response = await client.SendAsync(request);

            Response<ImageItem> result = await apiClient.ReadFromResponse<Response<ImageItem>>(response);

            if (response.IsSuccessStatusCode)
            {
                return result.Data.Id;
            }
            return 1;
        }

        public byte[] FromImageToBinary(string imagePath)
        {
            FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            return buffer;
        }
    }
}