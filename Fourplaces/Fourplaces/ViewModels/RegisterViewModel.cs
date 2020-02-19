using Common.Api.Dtos;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {

        private string _email;
        private string _firstName;
        private string _lastName;
        private string _password;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public Command ValidateRegistration { get; }


        public RegisterViewModel()
        {
            ValidateRegistration = new Command(createAccount);
        }

        public async void createAccount()
        {
            INavigationService navService = DependencyService.Get<INavigationService>();

            try
            {
                ApiClient apiClient = new ApiClient();
                HttpResponseMessage response = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/auth/register", new RegisterRequest() { Email = _email, FirstName = _firstName, LastName = _lastName, Password = _password });
                Response<RegisterRequest> result = await apiClient.ReadFromResponse<Response<RegisterRequest>>(response);

                bool success = result.IsSuccess;
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Succès", "Le compte a été créé", "Ok");
                    await navService.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Impossible de créer le compte", "Le compte n'a pas pu être créé", "Ok");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "Ok");
            }

        }
    }
}