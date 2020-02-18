using Common.Api.Dtos;
using Fourplaces.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        [NavigationParameter("userItem")]

        public UserItem userItem { get; set; }
        public ProfilePage profilePage { get; set; }

        private string _firstname;
        private string _lastname;
        private string _email;
        private string _currentPassword;
        private string _newPassword;
        private int _image_id;
        private string _imagePath;

        private string accessToken;
        private string originalPassword;

        public string FirstName
        {
            get => _firstname;
            set => SetProperty(ref _firstname, value);
        }

        public string LastName
        {
            get => _lastname;
            set => SetProperty(ref _lastname, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public int ImageId
        {
            get => _image_id;
            set => SetProperty(ref _image_id, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (SetProperty(ref _imagePath, value) && value != null) { }
            }
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public Command Validate { get; }
        public Command EditMode { get; }
        public Command TakePhoto { get; }
        public Command ChoosePicture { get; }

        public ProfileViewModel()
        {
            accessToken = App.Current.Properties["AccessToken"].ToString();
            originalPassword = App.Current.Properties["CurrentPassword"].ToString();

            EditMode = new Command(GoToEditMode);
            Validate = new Command(UpdateProfile);
            ChoosePicture = new Command(LoadPictureFromGallery);
            TakePhoto = new Command(OpenCamera);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            if (userItem != null)
            {
                FirstName = userItem.FirstName;
                LastName = userItem.LastName;
                Email = userItem.Email;
                ImageId = (int)userItem.ImageId;
            }
        }

        private void GoToEditMode()
        {
            ((Entry)profilePage.FindByName("FirstNameEntry")).IsEnabled = !((Entry)profilePage.FindByName("FirstNameEntry")).IsEnabled;
            ((Entry)profilePage.FindByName("LastNameEntry")).IsEnabled = !((Entry)profilePage.FindByName("LastNameEntry")).IsEnabled;
            ((StackLayout)profilePage.FindByName("ChangeImageLayout")).IsVisible = !((StackLayout)profilePage.FindByName("ChangeImageLayout")).IsVisible;
            ((StackLayout)profilePage.FindByName("PasswordsLayout")).IsVisible = !((StackLayout)profilePage.FindByName("PasswordsLayout")).IsVisible;
            ((Button)profilePage.FindByName("ValidateButton")).IsVisible = !((Button)profilePage.FindByName("ValidateButton")).IsVisible;
        }

        public async void UpdateProfile()
        {
            try
            {
                if (_imagePath != null && _imagePath != " ")
                {
                    ImageId = await SendNewImage();
                }

                //Maj des informations de base du profil
                if (!userItem.FirstName.Equals(FirstName) || !userItem.LastName.Equals(LastName) || !userItem.ImageId.Equals(ImageId))
                {
                    ApiClient apiClient = new ApiClient();
                    HttpResponseMessage response = await apiClient.Execute(new HttpMethod("PATCH"), "https://td-api.julienmialon.com/me", new UpdateProfileRequest()
                    {
                        FirstName = _firstname,
                        LastName = _lastname,
                        ImageId = _image_id
                    }, accessToken);
                    Response<UserItem> result = await apiClient.ReadFromResponse<Response<UserItem>>(response);

                    if (result.IsSuccess)
                    {
                        await Application.Current.MainPage.DisplayAlert("Succès", "Profil mis à jour", "Ok");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Echec", result.ErrorMessage, "Ok");
                    }
                }

                //Maj du mot de passe
                if ((!CurrentPassword.Equals(" ") || CurrentPassword != null) && (!NewPassword.Equals(" ") || NewPassword != null))
                {
                    if (CurrentPassword == originalPassword && CurrentPassword != NewPassword)
                    {
                        Console.WriteLine("Ils sont égaux");
                        ApiClient apiClient = new ApiClient();
                        HttpResponseMessage response = await apiClient.Execute(new HttpMethod("PATCH"), "https://td-api.julienmialon.com/me/password", new UpdatePasswordRequest()
                        {
                            OldPassword = _currentPassword,
                            NewPassword = _newPassword
                        }, accessToken);
                        Response<UpdatePasswordRequest> result = await apiClient.ReadFromResponse<Response<UpdatePasswordRequest>>(response);

                        if (result.IsSuccess)
                        {
                            await Application.Current.MainPage.DisplayAlert("Succès", "Mot de passe mis à jour", "Ok");
                            App.Current.Properties["CurrentPassword"] = _newPassword;
                            ((Entry)profilePage.FindByName("CurrentPasswordEntry")).Text = "";
                            ((Entry)profilePage.FindByName("NewPasswordEntry")).Text = "";
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Echec", "Le mot de passe n'a pas pu être mis à jour", "Ok");
                            ((Entry)profilePage.FindByName("CurrentPasswordEntry")).Text = "";
                            ((Entry)profilePage.FindByName("NewPasswordEntry")).Text = "";
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Erreur", "Le mot de passe actuel n'est pas le bon ou les mots de passe saisis sont identiques", "Ok");
                        ((Entry)profilePage.FindByName("CurrentPasswordEntry")).Text = "";
                        ((Entry)profilePage.FindByName("NewPasswordEntry")).Text = "";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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