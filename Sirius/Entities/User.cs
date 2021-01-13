using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirius.Entities
{
    public class User
    {
        //public String login { get; set; }
        public int ID { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<User> Friends { get; set; }

        public Rating rate(Series series, int stars, String comment)
        {
            return null;
        }

        public void befriend(User user)
        {

        }
    }
}
