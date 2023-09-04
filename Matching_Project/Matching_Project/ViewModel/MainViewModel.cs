using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Matching_Project.DAL;
using Matching_Project.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Usman.CodeBlocks.SQLiteManager;

namespace Matching_Project.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        LogDataDAL logDAL;
        private LogData _currentLeftItem;
        private LogData _currentRightItem;

        private ObservableCollection<LogData> _leftTable;
        private ObservableCollection<LogData> _rightTable;

        public LogData CurrentLeftItem { get => _currentLeftItem; set => SetProperty(ref _currentLeftItem, value); }
        public LogData CurrentRightItem { get => _currentRightItem; set => SetProperty(ref _currentRightItem, value); }

        public ObservableCollection<LogData> LeftTable { get => _leftTable; set => SetProperty(ref _leftTable, value); }
        public ObservableCollection<LogData> RightTable { get => _rightTable; set => SetProperty(ref _rightTable, value); }

        private ObservableCollection<string> _messages = new ObservableCollection<string>();
        public ObservableCollection<string> Messages
        {
            get => _messages;
            set => SetProperty(ref _messages, value);
        }


        public ICommand LeftTableSaveCommand { get; set; }
        public ICommand LeftTableDeleteCommand { get; set; }
        public ICommand RightTableSaveCommand { get; set; }
        public ICommand RightTableDeleteCommand { get; set; }
        public ICommand RightTableCopyCommand { get; set; }
        public ICommand LeftTableCopyCommand { get; set; }

        public MainViewModel()
        {
            RightTable = new ObservableCollection<LogData>();
            CurrentRightItem = new LogData();
            LeftTable = new ObservableCollection<LogData>();
            CurrentLeftItem = new LogData();

            LeftTableSaveCommand = new RelayCommand(ExecuteLeftTableSaveCommand);
            RightTableSaveCommand = new RelayCommand(ExecuteRightTableSaveCommand);
            //RightTableCopyCommand = new RelayCommand(ExecuteRightTableCopyCommand);
            //LeftTableCopyCommand = new RelayCommand(ExecuteLeftTableCopyCommand);
            LeftTableDeleteCommand = new RelayCommand<object>(new Action<object>(ExecuteLeftTableDeleteCommand));
            RightTableDeleteCommand = new RelayCommand<object>(new Action<object>(ExecuteRightTableDeleteCommand));

            logDAL = new LogDataDAL();


            SQLiteHelper.CreateTableIfNotExist<UserData>(new UserData());
            SQLiteHelper.CreateTableIfNotExist<LogData>(new LogData());

            var list = logDAL.Read(null);
            foreach (var item in list)
            {
                if (item.IsLeft)
                    LeftTable.Add(item);
                else
                    RightTable.Add(item);
            }

        }

        private void ExecuteLeftTableSaveCommand()
        {

            if (CurrentLeftItem.Age < 0 || CurrentLeftItem.Height < 0 || string.IsNullOrEmpty(CurrentLeftItem.President))
            {
                AddMessage("Invalid Data at LHS");
                return;

            }

            // Check if the data already exists in RightTable
            //var duplicate = LeftTable.FirstOrDefault(item =>
            //    item.Age == CurrentRightItem.Age &&
            //    item.President == CurrentRightItem.President &&
            //    item.Height == CurrentRightItem.Height);

            //if (duplicate != null)
            //{
            //    Messages.Insert(0, "same data is already exists");
            //    return;
            //}

            // 1.match left table data
            var matchingRightRow = RightTable.FirstOrDefault(item =>
                    item.Age == CurrentLeftItem.Age &&
                    item.Height == CurrentLeftItem.Height &&
                    item.President == CurrentLeftItem.President);

            if (matchingRightRow != null)
            {
                AddMessage("Match Found in RHS");
                RightTable.Remove(matchingRightRow);
                logDAL.DeleteEntryFromLogDAL(matchingRightRow);
                ClearField();
                return;

            }


            // 2. find difference of height
            matchingRightRow = RightTable.FirstOrDefault(item =>
                    item.Age == CurrentLeftItem.Age &&
                    item.President == CurrentLeftItem.President);

            if (matchingRightRow != null)
            {
                // if different is 1 then add in log
                var diff = Math.Abs(matchingRightRow.Height - CurrentLeftItem.Height);
                if (diff == 1)
                {
                    AddMessage(String.Format("President:{0}, Age{1}, Height Difference is {2}", CurrentLeftItem.President, CurrentLeftItem.Age, diff));
                }
            }


            CurrentLeftItem.IsLeft = true;
            var save = logDAL.Create(CurrentLeftItem);
            if (save)
            {
                LeftTable.Add(CurrentLeftItem);
                ClearField();
            }
            else
            {
                AddMessage("Failed to Submit");
            }
        }

        private void ExecuteRightTableSaveCommand()
        {

            if (CurrentRightItem.Age < 0 || CurrentRightItem.Height < 0 || string.IsNullOrEmpty(CurrentRightItem.President))
            {
                AddMessage("Invalid Data at RHS");
                return;
            }

            // Check if the data already exists in RightTable
            //var matchingRightRow = RightTable.FirstOrDefault(item =>
            //    item.Age == CurrentRightItem.Age &&
            //    item.President == CurrentRightItem.President &&
            //    item.Height == CurrentRightItem.Height);

            //if (matchingRightRow != null)
            //{
            //    Messages.Insert(0,"same data is already exists");
            //    return;
            //}

            // 1. match left table data
            var matchingLeftRow = LeftTable.FirstOrDefault(item =>
                    item.Age == CurrentRightItem.Age &&
                    item.Height == CurrentRightItem.Height &&
                    item.President == CurrentRightItem.President);

            if (matchingLeftRow != null)
            {
                AddMessage("Data Matched with LHS");
                LeftTable.Remove(matchingLeftRow);
                logDAL.DeleteEntryFromLogDAL(matchingLeftRow);
                ClearField();
                return;
                
            }


            // 2. find difference of height
            matchingLeftRow = LeftTable.FirstOrDefault(item =>
                    item.Age == CurrentRightItem.Age &&
                    item.President == CurrentRightItem.President);

            if (matchingLeftRow != null)
            {
                // if different is 1 then add in log
                var diff = Math.Abs(matchingLeftRow.Height - CurrentRightItem.Height);
                if (diff == 1)
                {
                    AddMessage(String.Format("President:{0}, Age{1}, Height Difference is {2}", CurrentRightItem.President, CurrentRightItem.Age, diff));
                }
            }

            CurrentRightItem.IsLeft = false;
            var save = logDAL.Create(CurrentRightItem);
            if (save)
            {
                RightTable.Add(CurrentRightItem);
                ClearField();
            }
            else
            {
                AddMessage("Failed to Submit");
            }
        }

        private void ExecuteRightTableDeleteCommand(object obj)
        {
            CurrentRightItem = (LogData)obj;


            var deleted = logDAL.Delete(CurrentRightItem.ID);
            if (deleted)
            {
                logDAL.Update(CurrentRightItem);
                RightTable.Remove(CurrentRightItem);
                ClearField();
            }
        }


        private void ExecuteLeftTableDeleteCommand(object obj)
        {
            CurrentLeftItem = (LogData)obj;



            var deleted = logDAL.Delete(CurrentLeftItem.ID);
            if (deleted)
            {

                logDAL.Update(CurrentLeftItem);
                LeftTable.Remove(CurrentLeftItem);
                ClearField();
            }

        }

        
        private void ClearField()
        {
            CurrentLeftItem = new LogData();
            CurrentRightItem = new LogData();
          
        }

        private void AddMessage(string message)
        {
            string msg = string.Format("{0} - {1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message);
            Messages.Insert(0,msg);
        }
    }
}
