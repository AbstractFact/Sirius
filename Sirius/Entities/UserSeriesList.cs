﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Entities
{
    public class UserSeriesList
    {
        public int ID { get; set; }
        public User User { get; set; }
        public Series Series { get; set; }
        public string Status { get; set; }
    }
}