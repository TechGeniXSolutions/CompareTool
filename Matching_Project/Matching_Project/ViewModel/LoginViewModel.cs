using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Matching_Project.DAL;
using Matching_Project.Models;
using Matching_Project.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Matching_Project.ViewModel
{

    public class LoginViewModel : ObservableObject
    {
        UserDAL fm = new UserDAL();
        public bool isCancelled;
        public bool userAuthenticated;
        UserData _user;



        public UserData User { get => _user; set => SetProperty(ref _user, value); }

        public ICommand LoginCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public LoginViewModel()
        {
            User = new UserData();
            fm = new UserDAL();
            LoginCommand = new RelayCommand(ExecuteLoginCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        private void ExecuteCancelCommand()
        {
            isCancelled = true;
            Application.Current.Shutdown();
        }

        private void ExecuteLoginCommand()
        {
           
        }  
    }
}

