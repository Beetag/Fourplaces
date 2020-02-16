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
        private double _latitude;
        private double _longitude;
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

        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public AddPlaceViewModel()
        {
            AddPlace = new Command(AddNewPlace);
        }

        public async void AddNewPlace()
        {
            try
            {
                ApiClient apiClient = new ApiClient();
                HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/places", new CreatePlaceRequest()
                {
                    Title = _title,
                    Description = _description,
                    ImageId = 1,
                    Latitude = _latitude,
                    Longitude = _longitude
                });
                Response<CreatePlaceRequest> result = await apiClient.ReadFromResponse<Response<CreatePlaceRequest>>(response);
                if (result.IsSuccess)
                {
                    Console.WriteLine(">>>>>>>>> Lieu ajouté");
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
    }
}