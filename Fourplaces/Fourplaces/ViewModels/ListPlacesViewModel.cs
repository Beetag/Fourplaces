using Common.Api.Dtos;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModel
{
    public class ListPlacesViewModel : ViewModelBase
    {
        private string _title;
        private string _image_id;
        private ObservableCollection<PlaceItemSummary> _listPlaceItemSummary;

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

        public Command GetAllPlaces { get; }

        public ListPlacesViewModel()
        {
            ListPlaceItemSummary = new ObservableCollection<PlaceItemSummary>();
            LoadAllPlaces();
        }

        private async void LoadAllPlaces()
        {
            ApiClient apiClient = new ApiClient();
            HttpResponseMessage response = await apiClient.Execute(HttpMethod.Get, "https://td-api.julienmialon.com/places");
            Response<ObservableCollection<PlaceItemSummary>> result = await apiClient.ReadFromResponse<Response<ObservableCollection<PlaceItemSummary>>>(response);

            if (result.IsSuccess)
            {
                Console.WriteLine("Reussi");
                ListPlaceItemSummary = result.Data;
            }
            foreach(PlaceItemSummary placeItem in ListPlaceItemSummary)
            {
                Console.WriteLine("test : " + placeItem.Title);

            }
        }
    }
}