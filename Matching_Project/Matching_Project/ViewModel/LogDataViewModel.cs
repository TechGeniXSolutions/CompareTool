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
   public class LogDataViewModel :ObservableObject
    {
        LogDataDAL LogData_db;
       LogData _logdata;
       
        public LogData Logdata { get=>_logdata; set=>SetProperty(ref _logdata,value); }


        public ICommand LogDataCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public LogDataViewModel()
        {
            Logdata = new LogData();
            LogData_db = new LogDataDAL();
            LogDataCommand = new RelayCommand(ExecuteLogDataCommand);

            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        private void ExecuteCancelCommand()
        {          
        }

        private void ExecuteLogDataCommand()
        {
           
            
        }
    
    }
}
