namespace Sirius.DTOs
{
    public class AwardedDTO
    {
        public int ID { get; set; }
        public AwardDTO Award { get; set; }
        public SeriesDTO Series { get; set; }
        public int Year { get; set; }
    }
}
