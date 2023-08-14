using Matching_Project.DAL;
using Matching_Project.Models;
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
        LoginFormDAL fm = new LoginFormDAL();

        public bool isCancelled;
        public bool userAuthenticated;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isCancelled = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // isCancelled = false;

            //if (txtUsername.Text.ToString().Equals("Admin"))
            //{
            //    if (txtPassword.Password.ToString().Equals("Admin"))
            //        userAuthenticated = true;
            //    else
            //    {
            //        MessageBox.Show("Invalid Password");
            //        txtPassword.Password = "";
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Invalid Username/Password");
            //    txtUsername.Text = "";
            //    txtPassword.Password = "";
            //}

            //    if (userAuthenticated)
            //        this.Close();
            //}
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            LoginFormDAL fm = new LoginFormDAL();
            List<LoginFormData> users = fm.Read(new LoginFormData());

            foreach (var user in users)
            {

                if (user.UserName == username && user.Password == password)
                {
                    MessageBox.Show("Login successful");
                    userAuthenticated = true;
                    this.Close();
                    return;
                }
            }

            MessageBox.Show("Invalid Username/Password");
        }
    }
    }
    

