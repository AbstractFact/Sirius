using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirius.Entities
{
    public class Series
    {
        public int ID { get; set; }
        public String Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public string Plot { get; set; }
        public int Seasons { get; set; }

        public float Rating { get; set; }
    }
}
