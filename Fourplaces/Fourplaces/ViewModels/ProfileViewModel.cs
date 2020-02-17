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

        private string _firtstname;
        private string _lastname;
        private string _email;
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

        public ProfileViewModel()
        {
                        
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            if (userItem != null)
            {
                FirstName = userItem.FirstName;
                LastName = userItem.LastName;
                Email = userItem.Email;
                //ImageId = userItem.ImageId;
            }
        }
    }
}