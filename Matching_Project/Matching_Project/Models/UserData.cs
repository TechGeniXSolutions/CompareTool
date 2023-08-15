using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matching_Project.Models
{
    public class UserData : BaseModel
    {
        private int _id;
        private string _username;
        private string _password;

        public int ID { get => _id; set => SetProperty(ref _id,value); }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
