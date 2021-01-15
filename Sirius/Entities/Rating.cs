using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirius.Entities
{
    public class Rating
    {
        public int ID { get; set; }
        public User User { get; set; }
        public Series Series { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
    }
}
