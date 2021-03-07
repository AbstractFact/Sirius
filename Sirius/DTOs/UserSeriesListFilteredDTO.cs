using Sirius.Entities;

namespace Sirius.DTOs
{
    public class UserSeriesListFilteredDTO
    {
        public int ID { get; set; }
        public int SeriesID { get; set; }
        public string SeriesTitle { get; set; }
        public string SeriesGenre { get; set; }
        public int SeriesSeasons { get; set; }
        public float SeriesRating { get; set; }
        public string Status { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; } 
        public bool  Favourite { get; set; }
    }
}
