using Common.Api.Dtos;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TD.Api.Dtos;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class DetailsPlaceViewModel : ViewModelBase
    {
        [NavigationParameter("placeItem")]
        public PlaceItem placeItem { get; set; }

        private string accessToken;
        private int _id;
        private string _title;
        private string _description;
        private double _latitude;
        private double _longitude;
        private int _image_id;
        private List<CommentItem> _listComments;
        public Command OpenMaps { get; }
        public Command AddComment { get; }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
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

        public int ImageId
        {
            get => _image_id;
            set => SetProperty(ref _image_id, value);
        }

        public List<CommentItem> ListComments
        {
            get => _listComments;
            set => SetProperty(ref _listComments, value);
        }

        public DetailsPlaceViewModel()
        {
            accessToken = App.Current.Properties["AccessToken"].ToString();

            OpenMaps = new Command(GoToMaps);
            AddComment = new Command(OpenCommentPopUp);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            if (placeItem != null)
            {
                Id = placeItem.Id;
                Title = placeItem.Title;
                Description = placeItem.Description;
                ImageId = placeItem.ImageId;
                Latitude = placeItem.Latitude;
                Longitude = placeItem.Longitude;
                ListComments = placeItem.Comments;
            }
            else
            {
                Title = "Error";
            }
        }

        private async void GoToMaps()
        {
            Console.WriteLine("Latitude : " + Latitude + " Longitude : " + Longitude);
            Location location = new Location(Latitude, Longitude);
            MapLaunchOptions options = new MapLaunchOptions { Name = Title };
            await Map.OpenAsync(location, options);

        }

        private async void OpenCommentPopUp()
        {
            string comment = await App.Current.MainPage.DisplayPromptAsync("Ajouter un commentaire", null, "Ajouter", "Annuler");
            Debug.WriteLine("Commentaire : " + comment);

            if (comment != null || comment != "")
            {
                try
                {
                    ApiClient apiClient = new ApiClient();
                    HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/places/" + Id + "/comments", new CreateCommentRequest() 
                    {
                        Text = comment
                    }, accessToken);
                    Response<CreateCommentRequest> result = await apiClient.ReadFromResponse<Response<CreateCommentRequest>>(response);

                    if (result.IsSuccess)
                    {
                        await Application.Current.MainPage.DisplayAlert("Succès", "Commentaire ajouté", "Ok");
                        UpdateComments(Id);
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

        private async void UpdateComments(int placeItemSummaryId)
        {
            try
            {
                ApiClient apiClient = new ApiClient();
                HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Get, "https://td-api.julienmialon.com/places/" + placeItemSummaryId);
                Response<PlaceItem> result = await apiClient.ReadFromResponse<Response<PlaceItem>>(httpResponse);

                if (result.IsSuccess)
                {
                    ListComments.Clear();
                    ListComments = result.Data.Comments;
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "Ok");
            }
        }

    }
}