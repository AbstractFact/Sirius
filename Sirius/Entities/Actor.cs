using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirius.Entities
{
    public class Actor
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Birthplace { get; set; }
        public String Birthday { get; set; }
        public String Biography { get; set; }

        public DateTime getBirthday()
        {
            if(this.Birthday == null) return new DateTime();

            long timestamp = Int64.Parse(this.Birthday);
            DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return startDateTime.AddMilliseconds(timestamp).ToLocalTime();
        }

        public List<Series> filmography { get; set; }

        public Role playedIn(Series movie, String role)
        {
            return null;
        }
    }
}
