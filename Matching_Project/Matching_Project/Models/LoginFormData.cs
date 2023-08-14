using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matching_Project.Models
{
   public class LoginFormData :ObservableObject
    {
        public int _id;
        public string _passWord;
        public string _userName;
        public string Password { get=> _passWord; set=>SetProperty(ref _passWord, value); }
        public int ID { get=>_id; set=>SetProperty(ref _id,value); }
        public string UserName { get=> _userName; set=>SetProperty(ref _userName, value); }
    }
}
