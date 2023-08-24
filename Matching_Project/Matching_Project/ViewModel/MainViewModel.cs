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
           // LeftTableDeleteCommand = new RelayCommand<object>(new Action<object>(ExecuteLeftTableDeleteCommand));
           // RightTableDeleteCommand = new RelayCommand<object>(new Action<object>(ExecuteRightTableDeleteCommand));

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

        //private void ExecuteLeftTableCopyCommand()
        //{
        //    StringBuilder copyData = new StringBuilder();
        //    // Adding header row
        //    copyData.AppendLine("Age\tHeight\tPresident");

        //    foreach (var item in LeftTable)
        //    {
        //        copyData.AppendLine($"{item.Age}\t{item.Height}\t{item.President}");

        //    }

        //    Clipboard.SetText(copyData.ToString(), TextDataFormat.Text);

        //    Messages.Add("data copied successfully");
        //}

        //private void ExecuteRightTableCopyCommand()
        //{
        //    StringBuilder copyData = new StringBuilder();
        //    // Adding header row
        //    copyData.AppendLine("President\tHeight\tAge");

        //    foreach (var leftTableItem in RightTable)
        //    {
        //        copyData.AppendLine($"{leftTableItem.Age}\t{leftTableItem.Height}\t{leftTableItem.President}");

        //    }

        //    Clipboard.SetText(copyData.ToString(), TextDataFormat.Text);

        //    Messages.Add("data copied successfully");
        //}

        private void ExecuteRightTableSaveCommand()
        {
            //int totalRightRows = RightTable.Count();
            //int totalLeftRows = LeftTable.Count();

            //if (totalLeftRows <= 0)
            //{
            //    MessageBox.Show("Not enough Rows to compare");
            //    return;
            //}

            // Check if the data already exists in RightTable
            var matchingRightRow = RightTable.FirstOrDefault(item =>
                item.Age == CurrentRightItem.Age &&
                item.President == CurrentRightItem.President &&
                item.Height == CurrentRightItem.Height);

            if (matchingRightRow != null)
            {
                Messages.Add("same data is already exists");
                return;
            }

            var matchingLeftRow = LeftTable.FirstOrDefault(item =>
                    item.Age == CurrentRightItem.Age &&
                    item.President == CurrentRightItem.President);

            if (matchingLeftRow != null)
            {
                var diff = Math.Abs(matchingLeftRow.Height - CurrentRightItem.Height);
                if (diff == 1)
                {
                    Messages.Add(String.Format("President:{0}, Age{1}, Height Difference is {2}", CurrentRightItem.President, CurrentRightItem.Age, diff));
                }
                else if (diff == 0)
                {
                    // Remove matching row
                    LeftTable.Remove(matchingLeftRow);
                    logDAL.DeleteEntryFromLogDAL(matchingLeftRow);
                    Messages.Add("Matching data found");
                    return;
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
                Messages.Add("Failed to save");
            }
        }

        //private void ExecuteRightTableDeleteCommand(object obj)
        //{
        //    CurrentRightItem = (LogData)obj;


        //    var deleted = logDAL.Delete(CurrentRightItem.ID);
        //    if (deleted)
        //    {
        //        logDAL.Update(CurrentRightItem);
        //       // Messages.Add("record is deleted successfully");
        //        ClearField();
        //    }


        //    else

        //    {
        //        MessageBox.Show("failed to process your Action");
        //    }
        //}

      
        //private void ExecuteLeftTableDeleteCommand(object obj)
        //{
        //    CurrentLeftItem = (LogData)obj;

           

        //        var deleted = logDAL.Delete(CurrentLeftItem.ID);
        //        if (deleted)
        //        {
                
        //            logDAL.Update(CurrentLeftItem);
        //           // MessageBox.Show("record is deleted successfully");
        //            ClearField();
        //        }

            
        //    else
        //    {
        //        MessageBox.Show("failed to process your Action");
        //    }

        //}
   
        private void ExecuteLeftTableSaveCommand()
        {
            if (LeftTable.Any(item => item.Age == CurrentLeftItem.Age && item.Height == CurrentLeftItem.Height && item.President == CurrentLeftItem.President))
            {
                Messages.Add("Invalid: Duplicate data is not allowed");
                
            }
            
            else
            {
                CurrentLeftItem.IsLeft = true;
                var save = logDAL.Create(CurrentLeftItem);
                if (save)
                {
                    LeftTable.Add(CurrentLeftItem);
                    ClearField();
                    //MessageBox.Show("data save successfully");

                }
                else
                {
                    Messages.Add("Failed to save");
                    ClearField();
                }
            }
        }
        private void ClearField()
        {
            CurrentLeftItem = new LogData();
            CurrentRightItem = new LogData();
          
        }
    }
}
