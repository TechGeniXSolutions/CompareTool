using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matching_Project.Models
{
   public class LeftTable:ObservableObject
    {
        private int _id;
        private int _age;
        public int _height;
        public string _president;
        public int Age { get => _id; set => SetProperty(ref _id, value); }
        public int ID { get => _age; set => SetProperty(ref _age, value); }

        public int Height { get => _height; set => SetProperty(ref _height, value); }

        public string President { get => _president; set => SetProperty(ref _president, value); }
    }
}
