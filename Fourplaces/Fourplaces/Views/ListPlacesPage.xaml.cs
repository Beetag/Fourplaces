using Fourplaces.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fourplaces
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListPlacesPage : ContentPage
    {
        public ListPlacesPage()
        {
            BindingContext = new ListPlacesViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var listPlaceViewModel = BindingContext as ListPlacesViewModel;
            listPlaceViewModel.LoadAllPlaces();
        }
    }
}