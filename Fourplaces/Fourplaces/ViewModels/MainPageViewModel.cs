using Common.Api.Dtos;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System.Net.Http;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        private string _eMail;
        private string _password;

        public string Email
        {
            get => _eMail;
            set => SetProperty(ref _eMail, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public Command GoToRegister { get; }
        public Command Login { get; } 

        public MainPageViewModel() 
        {
            Login = new Command(connection_button_Clicked);
            //goToRegister = new Command(connection_button_Clicked);
        }
        private async void connection_button_Clicked()
        {
            INavigationService navService = DependencyService.Get<INavigationService>();

            ApiClient apiClient = new ApiClient();
            HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/auth/login", new LoginRequest() { Email = _eMail, Password = _password });
            Response<LoginResult> result = await apiClient.ReadFromResponse<Response<LoginResult>>(response);

            bool success = result.IsSuccess;

            if (success)
            {
                await navService.PushAsync(new ListPlacesPage());
            }
        }
    }
}
