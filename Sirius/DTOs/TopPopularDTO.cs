using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.DTOs
{
    public class TopPopularDTO
    { 
        public int SeriesID { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public float Rating { get; set; }
        public int Popularity { get; set; }
    }
}
