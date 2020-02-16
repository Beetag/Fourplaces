using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private int _id;
        private string _title;
        private string _description;
        private double _latitude;
        private double _longitude;
        private int _image_id;
        private List<CommentItem> _listComments;
        public Command OpenMaps { get; }

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
            OpenMaps = new Command(openMaps);
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

        private async void openMaps()
        {
            Console.WriteLine("Latitude : " + Latitude + " Longitude : " + Longitude);
            Location location = new Location(Latitude, Longitude);
            MapLaunchOptions options = new MapLaunchOptions { Name = Title };
            await Map.OpenAsync(location, options);

        }

    }
}