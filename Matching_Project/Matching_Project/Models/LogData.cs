﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matching_Project.Models
{
    public class LogData : ObservableObject
    {
        private int _id;
        private int _age;
        private int _height;
        private string _president;
        private bool _isleft;

        public int ID { get => _id; set => SetProperty(ref _id,value); }
        public int Age { get=>_age; set=>SetProperty(ref _age,value); }
        public int Height { get=>_height; set=>SetProperty(ref _height,value); }
        public string President { get=>_president; set=>SetProperty(ref _president,value); }
        public bool IsLeft { get=>_isleft; set=>SetProperty(ref _isleft,value); }

    }
}