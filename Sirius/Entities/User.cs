using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirius.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
