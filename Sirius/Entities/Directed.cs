using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Entities
{
    public class Directed
    {
        public int ID { get; set; }
        public Series Series { get; set; }
        public Person Director { get; set; }
    }
}
