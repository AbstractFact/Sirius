namespace Sirius.DTOs
{
    public class UserSeriesListDTO
    {
        public int ID { get; set; }
        public UserDTO User { get; set; }
        public SeriesDTO Series { get; set; }
        public string Status { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public bool Favourite { get; set; }
    }
}
