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
        public List<Role> Cast { get; set; }
    }
}
