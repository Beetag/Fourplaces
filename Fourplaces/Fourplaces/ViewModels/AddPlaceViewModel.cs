using Common.Api.Dtos;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class AddPlaceViewModel : ViewModelBase
    {
        private string _title;
        private string _description;
        private int _image_id;
        private string _latitude;
        private string _longitude;
        private string accessToken;
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

        public AddPlaceViewModel()
        {
            AddPlace = new Command(AddNewPlace);
            UseMyCoordinates = new Command(GetMyCoordinates);
            accessToken = App.Current.Properties["AccessToken"].ToString();
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

            try
            {
                ApiClient apiClient = new ApiClient();
                HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/places", new CreatePlaceRequest()
                {
                    Title = _title,
                    Description = _description,
                    ImageId = 1,
                    Latitude = double.Parse(_latitude),
                    Longitude = double.Parse(_longitude)
                }, accessToken);
                Response<CreatePlaceRequest> result = await apiClient.ReadFromResponse<Response<CreatePlaceRequest>>(response);

                if (result.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert("Succès", "Nouveau lieu ajouté", "Ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", result.ErrorMessage, "Ok");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "Ok");
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
    }
}