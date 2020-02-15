using Common.Api.Dtos;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModel
{
    public class ListPlacesViewModel : ViewModelBase
    {
        private string _placeName;
        private string _placeDescription;
        private string _placeURLImage;

        public string placeName
        {
            get => _placeName;
            set => SetProperty(ref _placeName, value);
        }

        public string placeDescription
        {
            get => _placeDescription;
            set => SetProperty(ref _placeDescription, value);
        }

        public string placeURLImage
        {
            get => _placeURLImage;
            set => SetProperty(ref _placeURLImage, value);
        }
        public ListPlacesViewModel()
        {

        }

        private async void connection_button_Clicked()
        {
            ApiClient apiClient = new ApiClient();
            HttpResponseMessage response = await apiClient.Execute(HttpMethod.Get, "https://td-api.julienmialon.com/places", new PlaceItemSummary());
            Response<List<PlaceItemSummary>> result = await apiClient.ReadFromResponse<Response<List<PlaceItemSummary>>>(response);

            bool success = result.IsSuccess;

            if (success)
            {

            }
    }
}