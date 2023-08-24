using Matching_Project.Models;
using Matching_Project.ViewModel;
using Matching_Project.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Usman.CodeBlocks.SQLiteManager;

namespace Matching_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //MainViewModel vm = new MainViewModel();
        //LefttableViewModel LV = new LefttableViewModel();
        
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel vm = new MainViewModel();
            this.DataContext = vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginForm lg = new LoginForm();
            this.Hide();
            lg.WindowState = WindowState.Maximized;
            //lg.DataContext = this;
            lg.ShowDialog();
            if (lg.isCancelled == true)
            {
                this.Close();
                Application.Current.Shutdown();
            }
            else
            {
                if (lg.userAuthenticated == true)
                    this.Show();
                else
                {
                    this.Close();
                    Application.Current.Shutdown();
                }
            }
        }

        private void txtLeftAge_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLeftAge.SelectAll();
        }

        private void txtLeftHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLeftHeight.SelectAll();
        }

      

        private void txtLeftPresident_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLeftPresident.SelectAll();
        }

        private void txtRightPresident_GotFocus(object sender, RoutedEventArgs e)
        {
            txtRightPresident.SelectAll();
        }

        private void txtRightHight_GotFocus(object sender, RoutedEventArgs e)
        {
            txtRightHight.SelectAll();
        }

        private void txtRightAge_GotFocus(object sender, RoutedEventArgs e)
        {
            txtRightAge.SelectAll();
        }
    }
}
