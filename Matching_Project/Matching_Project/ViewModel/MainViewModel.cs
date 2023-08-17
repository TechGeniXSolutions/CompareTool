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
      private  LogData _currentLeftItem;
      private  LogData _currentRightItem;

      private  ObservableCollection<LogData> _leftTable;
      private  ObservableCollection<LogData> _rightTable;

        public LogData CurrentLeftItem { get=>_currentLeftItem; set=>SetProperty(ref _currentLeftItem,value); }
        public LogData CurrentRightItem { get=>_currentRightItem; set=>SetProperty(ref _currentRightItem,value); }

        public ObservableCollection<LogData> LeftTable { get=>_leftTable; set=>SetProperty(ref _leftTable,value); }
        public ObservableCollection<LogData> RightTable { get=>_rightTable; set=>SetProperty(ref _rightTable,value); }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

      
        public ICommand LeftTableSaveCommand { get; set; }
        public ICommand LeftTableDeleteCommand { get; set; }
        public ICommand RightTableSaveCommand { get; set; }
        public ICommand RightTableDeleteCommand { get; set; }

        public MainViewModel()
        {
            RightTable = new ObservableCollection<LogData>();
            CurrentRightItem = new LogData();
            LeftTable = new ObservableCollection<LogData>();
            CurrentLeftItem = new LogData();

            LeftTableSaveCommand = new RelayCommand(ExecuteLeftTableSaveCommand);
            RightTableSaveCommand = new RelayCommand(ExecuteRightTableSaveCommand);

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

        private void ExecuteRightTableSaveCommand()
        {

            if (CurrentRightItem.ID <= 0)
            {
                int totalRightRows = RightTable.Count();
                int totalLeftRows = LeftTable.Count();
                if (totalLeftRows <= totalRightRows)
                {
                    MessageBox.Show("Not enough Rows to compare");
                    return;
                }

                var leftrow = LeftTable[totalRightRows];

                if (leftrow.Age == CurrentRightItem.Age && leftrow.President == CurrentRightItem.President)
                {
                    if (leftrow.Height == CurrentRightItem.Height)
                    {
                        // remove row from right column


                       // LeftTable.RemoveAt(totalRightRows);
                        // Remove row from left table
                        LeftTable.RemoveAt(totalRightRows);

                        // Delete corresponding entry from logDAL
                        logDAL.DeleteEntryFromLogDAL(leftrow); // 
                        SuccessMessage = "both tables have same height";

                        // Delete corresponding entry from logDAL
                       

                    }
                    else
                    {
                        int diff = leftrow.Height - CurrentRightItem.Height;
                        diff = Math.Abs(diff);
                        if (diff == 1)
                        {
                            SuccessMessage = "difference is one";
                        }
                        else
                        {
                            //save row data in righttable
<<<<<<< Updated upstream
                          
=======
                            tableRight.Create(CurrentValue);
>>>>>>> Stashed changes
                        }
                    }
                }
                else
                {
                    CurrentRightItem.IsLeft = false;
                    var save = logDAL.Create(CurrentRightItem);
                    if (save)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(SuccessMessage = "Data added successfully");
                        });
                        RightTable.Add(CurrentRightItem);
                        ClearField();
                    }
                    else
                    {
                        SuccessMessage = "Failed to save";
                    }
                }
            }

        }


        private void ExecuteRightTableDeleteCommand(object obj)
        {
            CurrentRightItem = (LogData)obj;

          
                var deleted = logDAL.Delete(CurrentRightItem.ID);
                if (deleted)
                {
                    logDAL.Update(CurrentRightItem);
                    MessageBox.Show("record is deleted successfully");
                    ClearField();
                }

            
            else

            {
                MessageBox.Show("failed to process your Action");
            }
        }

        private void ExecuteRightTableEditCommand(object obj)
        {
            CurrentRightItem = (LogData)obj;
           logDAL.Update(CurrentRightItem);
         


        }

        private void ExecuteLeftTableDeleteCommand(object obj)
        {
            CurrentLeftItem = (LogData)obj;

           

                var deleted = logDAL.Delete(CurrentLeftItem.ID);
                if (deleted)
                {
                    logDAL.Update(CurrentLeftItem);
                    MessageBox.Show("record is deleted successfully");
                    ClearField();
                }

            
            else
            {
                MessageBox.Show("failed to process your Action");
            }

        }

        private void ExecuteLeftTableSaveCommand()
        {
            if (LeftTable.Any(item => item.Age == CurrentLeftItem.Age && item.Height == CurrentLeftItem.Height && item.President == CurrentLeftItem.President))
            {
                SuccessMessage = "Invalid: Duplicate data is not allowed";
                
            }
            else
            {
                CurrentLeftItem.IsLeft = true;
                var save = logDAL.Create(CurrentLeftItem);
                if (save)
                {
                    LeftTable.Add(CurrentLeftItem);
                    MessageBox.Show("data save successfully");
                    
                }
                else
                {
                    SuccessMessage = "Failed to save";
                    ClearField();
                }
            }
        }
        private void ClearField()
        {
            CurrentLeftItem = new LogData();
            CurrentRightItem = new LogData();
            SuccessMessage = string.Empty;
        }
    }
}
