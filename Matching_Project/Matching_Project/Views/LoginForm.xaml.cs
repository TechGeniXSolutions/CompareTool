using Matching_Project.DAL;
using Matching_Project.Models;
using Matching_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Windows.Shapes;

namespace Matching_Project.Views
{

    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        UserDAL dal = new UserDAL();

        public bool isCancelled;
        public bool userAuthenticated;

        public LoginForm()
        {
            InitializeComponent();
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            var user = dal.Login(username, password);
            if (user != null)
            {
              
                userAuthenticated = true;
                this.Close();
                return;
            }

           // MessageBox.Show("Invalid Username/Password");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            isCancelled = true;
            this.Close();
        }
    }
    }
    

