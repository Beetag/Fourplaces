using Common.Api.Dtos;
using Fourplaces.Views;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModel
{
    public class ListPlacesViewModel : ViewModelBase
    {
        private string accessToken;
        private string _title;
        private string _image_id;
        private ObservableCollection<PlaceItemSummary> _listPlaceItemSummary;
        private PlaceItemSummary _selectedPlaceItemSummary;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string ImageId
        {
            get => _image_id;
            set => SetProperty(ref _image_id, value);
        }

        public ObservableCollection<PlaceItemSummary> ListPlaceItemSummary
        {
            get => _listPlaceItemSummary;
            set
            {
                if (SetProperty(ref _listPlaceItemSummary, value) && value != null)
                {
                    _listPlaceItemSummary = value;
                    OnPropertyChanged();
                }
            }

        }

        public PlaceItemSummary SelectedPlaceItemSummary
        {
            get => _selectedPlaceItemSummary;
            set
            {
                if (SetProperty(ref _selectedPlaceItemSummary, value) && value != null)
                {
                    GoToDetailsPage(value);
                    SelectedPlaceItemSummary = null;
                }
            }
        }

        public Command GetAllPlaces { get; }
        public Command NewPlace { get; }
        public Command GoToProfile { get; }

        public ListPlacesViewModel()
        {
            ListPlaceItemSummary = new ObservableCollection<PlaceItemSummary>();
            NewPlace = new Command(CreateNewPlace);
            GoToProfile = new Command(GoToMyProfile);
            accessToken = App.Current.Properties["AccessToken"].ToString();
            LoadAllPlaces();
        }

        private async void LoadAllPlaces()
        {
            ApiClient apiClient = new ApiClient();
            HttpResponseMessage response = await apiClient.Execute(HttpMethod.Get, "https://td-api.julienmialon.com/places");
            Response<ObservableCollection<PlaceItemSummary>> result = await apiClient.ReadFromResponse<Response<ObservableCollection<PlaceItemSummary>>>(response);

            if (result.IsSuccess)
            {
                ListPlaceItemSummary = result.Data;
            }
        }

        private async void GoToDetailsPage(PlaceItemSummary placeItemSummary)
        {
            try
            {
                ApiClient apiClient = new ApiClient();
                HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Get, "https://td-api.julienmialon.com/places/" + placeItemSummary.Id);
                Response<PlaceItem> result = await apiClient.ReadFromResponse<Response<PlaceItem>>(httpResponse);

                if (result.IsSuccess)
                {
                    Console.Write("Succes récupération");
                    await DependencyService.Get<INavigationService>().PushAsync<DetailsPlacePage>(new Dictionary<string, object> {
                        { "placeItem" , result.Data }
                    });
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "Ok");
            }
        }

        private async void GoToMyProfile()
        {
            try
            {
                ApiClient apiClient = new ApiClient();
                HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Get, "https://td-api.julienmialon.com/me", null, accessToken);
                Response<UserItem> result = await apiClient.ReadFromResponse<Response<UserItem>>(httpResponse);

                if (result.IsSuccess)
                {
                    await DependencyService.Get<INavigationService>().PushAsync<ProfilePage>(new Dictionary<string, object> {
                        { "userItem" , result.Data }
                    });
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "Ok");
            }
        }

        private async void CreateNewPlace()
        {
            await DependencyService.Get<INavigationService>().PushAsync<AddPlacePage>();
        }
    }
}