using Common.Api.Dtos;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TD.Api.Dtos;
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
            accessToken = App.Current.Properties["AccessToken"].ToString();
        }

        public async void AddNewPlace()
        {
            if (!Title.Equals(""))
            {
                if (Latitude.Equals("") || Longitude.Equals(""))
                try
                {
                    ApiClient apiClient = new ApiClient();
                    HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/places", new CreatePlaceRequest()
                    {
                        Title = _title,
                        Description = _description,
                        ImageId = 1,
                        Latitude = Convert.ToDouble(_latitude),
                        Longitude = Convert.ToDouble(_longitude)
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
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez renseigner un nom", "Ok");
            }
        }
    }
}