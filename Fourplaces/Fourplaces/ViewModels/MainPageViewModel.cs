using Common.Api.Dtos;
using Fourplaces.Views;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
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
            Login = new Command(login);
            GoToRegister = new Command(register);
        }

        private async void login()
        {
            INavigationService navService = DependencyService.Get<INavigationService>();

            try
            {
                ApiClient apiClient = new ApiClient();
                HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/auth/login", new LoginRequest() { Email = _eMail, Password = _password });
                Response<LoginResult> result = await apiClient.ReadFromResponse<Response<LoginResult>>(response);

                bool success = result.IsSuccess;

                if (success)
                {
                    App.Current.Properties["AccessToken"] = result.Data.AccessToken;
                    App.Current.Properties["CurrentPassword"] = _password;

                    await navService.PushAsync(new ListPlacesPage());
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Identifiants invalides", "Ok");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "Ok");
            }
        }

        private async void register()
        {
            await DependencyService.Get<INavigationService>().PushAsync<RegisterPage>();
        }
    }
}
