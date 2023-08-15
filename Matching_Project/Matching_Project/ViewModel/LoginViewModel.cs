using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Matching_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Matching_Project.ViewModel
{
    
   public class LoginViewModel : ObservableObject
   { 
     
        UserData _user;
       

        public UserData User { get => _user; set => SetProperty(ref _user,value); }

        public ICommand LoginCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public LoginViewModel()
        {
            User = new UserData();
            LoginCommand = new RelayCommand(ExecuteLoginCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        private void ExecuteCancelCommand()
        {
          
        }

        private void ExecuteLoginCommand()
        {
            
        }
    }
}
