using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.DTOs
{
    public class NewSeriesNotificationDTO
    {
        public int ID { get; set; }
        public int SeriesID { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
    }
}
