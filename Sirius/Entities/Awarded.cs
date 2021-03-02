using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Entities
{
    public class Awarded
    {
        public int ID { get; set; }
        public Award Award { get; set; }
        public Series Series { get; set; }
        public int Year { get; set; }
    }
}
