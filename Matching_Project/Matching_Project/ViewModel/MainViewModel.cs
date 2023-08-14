using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HIKDataTool.DAL;
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
        RightTableDAL tableRight;
        leftTableDAL tableLeft;
        private RightTable _currentValue;
        private ObservableCollection<LeftTable> _leftItems;
        public RightTable CurrentValue { get => _currentValue; set => SetProperty(ref _currentValue, value); }
        public ObservableCollection<LeftTable> LeftItems { get => _leftItems; set => SetProperty(ref _leftItems, value); }
        private LeftTable _currentItem;
        private ObservableCollection<RightTable> _rightitems;
        public LeftTable CurrentItem { get => _currentItem; set => SetProperty(ref _currentItem, value); }
        public ObservableCollection<RightTable> RightItems { get => _rightitems; set => SetProperty(ref _rightitems, value); }
        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        public ICommand LeftTableSaveCommand { get; set; }


        public ICommand LeftTableEditCommand { get; set; }
        public ICommand LeftTableDeleteCommand { get; set; }
        public ICommand RightTableSaveCommand { get; set; }
        public ICommand RightTableEditCommand { get; set; }
        public ICommand RightTableDeleteCommand { get; set; }

        public MainViewModel()
        {
            RightItems = new ObservableCollection<RightTable>();
            CurrentValue = new RightTable();

            LeftTableSaveCommand = new RelayCommand(ExecuteLeftTableSaveCommand);
            LeftTableEditCommand = new RelayCommand<object>(new Action<object>(ExecuteLeftTableEditCommand));
            LeftTableDeleteCommand = new RelayCommand<object>(new Action<object>(ExecuteLeftTableDeleteCommand));
            RightTableEditCommand = new RelayCommand<object>(new Action<object>(ExecuteRightTableEditCommand));
            RightTableDeleteCommand = new RelayCommand<object>(new Action<object>(ExecuteRightTableDeleteCommand));
            LeftItems = new ObservableCollection<LeftTable>();
            CurrentItem = new LeftTable();
            tableLeft = new leftTableDAL();
            tableRight = new RightTableDAL();

            RightTableSaveCommand = new RelayCommand(ExecuteRightTableSaveCommand);
            SQLiteHelper.CreateTableIfNotExists();
            SQLiteHelper.CreateTableIfNotExist<RightTable>(new RightTable());
            SQLiteHelper.CreateTableIfNotExist<LeftTable>(new LeftTable());
            SQLiteHelper.CreateTableIfNotExist<LoginFormData>(new LoginFormData());
            var list = tableRight.Read(null);
            foreach (var item in list)
            {
                RightItems.Add(item);
            }
            var li = tableLeft.Read(null);
            foreach (var items in li)
            {
                LeftItems.Add(items);
            }

        }

        private void ExecuteRightTableSaveCommand()
        {

            if (CurrentValue.ID <= 0)
            {
                int totalRightRows = RightItems.Count();
                int totalLeftRows = LeftItems.Count();
                if (totalLeftRows <= totalRightRows)
                {
                    MessageBox.Show("Not enough Rows to compare");
                    return;
                }

                var leftrow = LeftItems[totalRightRows];

                if (leftrow.Age == CurrentValue.Age && leftrow.President == CurrentValue.President)
                {
                    if (leftrow.Height == CurrentValue.Height)
                    {
                        // remove row from right column
                         LeftItems.Remove(leftrow);

                    }
                    else
                    {
                        int diff = leftrow.Height - CurrentValue.Height;
                        diff = Math.Abs(diff);
                        if (diff == 1)
                        {
                            SuccessMessage = "difference is one";
                        }
                        else
                        {
                            //save row data in righttable
                        }
                    }
                }
                else
                {
                    var save = tableRight.Create(CurrentValue);
                    if (save)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(SuccessMessage = "Data added successfully");
                        });
                        RightItems.Add(CurrentValue);
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
            CurrentValue = (RightTable)obj;

          
                var deleted = tableRight.Delete(CurrentValue.ID);
                if (deleted)
                {
                    tableRight.Update(CurrentValue);
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
            CurrentValue = (RightTable)obj;
           tableRight.Update(CurrentValue);
           
            
        }

        private void ExecuteLeftTableDeleteCommand(object obj)
        {
            CurrentItem = (LeftTable)obj;

           

                var deleted = tableLeft.Delete(CurrentItem.ID);
                if (deleted)
                {
                    tableLeft.Update(CurrentItem);
                    MessageBox.Show("record is deleted successfully");
                    ClearField();
                }

            
            else

            {
                MessageBox.Show("failed to process your Action");
            }

        }

        private void ExecuteLeftTableEditCommand(object obj)
        {
            try
            {
                CurrentItem = (LeftTable)obj;
                tableLeft.Update(CurrentItem);
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error editing data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    

        private void ExecuteLeftTableSaveCommand()
        {
            if (CurrentItem.ID <= 0)
            {
                
                if (LeftItems.Any(item => item.Age == CurrentItem.Age && item.Height == CurrentItem.Height && item.President == CurrentItem.President))
                {
                    SuccessMessage = "Invalid: Duplicate data is not allowed";
                }
                else
                {
                    var save = tableLeft.Create(CurrentItem);
                    if (save)
                    {
                        LeftItems.Add(CurrentItem);
                        MessageBox.Show("data save successfully");
                        ClearField();
                    }
                    else
                    {
                        SuccessMessage = "Failed to save";
                    }
                }
            }
            else
            {

                tableLeft.Update(CurrentItem);

            }
        }
        private void ClearField()
        {
            CurrentItem = new LeftTable();
            CurrentValue = new RightTable();
            SuccessMessage = string.Empty;
        }
    }
}
