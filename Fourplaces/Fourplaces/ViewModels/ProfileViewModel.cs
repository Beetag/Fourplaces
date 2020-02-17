using Fourplaces.Views;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        [NavigationParameter("userItem")]

        public UserItem userItem {get; set;}
        public ProfilePage profilePage { get; set; }

        private string _firtstname;
        private string _lastname;
        private string _email;
        private string _currentPassword;
        private string _newPassword;
        private int _image_id;

        public string FirstName
        {
            get => _firtstname;
            set => SetProperty(ref _firtstname, value);
        }

        public string LastName
        {
            get => _lastname;
            set => SetProperty(ref _lastname, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public int ImageId
        {
            get => _image_id;
            set => SetProperty(ref _image_id, value);
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public Command Validate { get; }
        public Command EditMode { get; }

        public ProfileViewModel()
        {
            EditMode = new Command(GoToEditMode);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            if (userItem != null)
            {
                FirstName = userItem.FirstName;
                LastName = userItem.LastName;
                Email = userItem.Email;
                ImageId = (int)userItem.ImageId;
            }
        }

        private void GoToEditMode()
        {
            //((Entry)profilePage.FindByName("FirstName")).IsEnabled = !((Entry)profileView.FindByName("EmailEntry")).IsEnabled;
        }
    }
}