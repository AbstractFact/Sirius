using Sirius.Entities;

namespace Sirius.DTOs
{
    public class UserSeriesListFilteredDTO
    {
        public int ID { get; set; }
        public Series Series { get; set; }
        public string Status { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; } 
        public bool  Favourite { get; set; }
    }
}
