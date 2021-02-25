using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.DTOs
{
    public class FavouriteSeriesEditDTO
    {
        public string Status { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public bool Favourite { get; set; }
    }
}
