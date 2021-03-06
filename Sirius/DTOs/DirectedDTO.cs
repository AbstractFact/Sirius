namespace Sirius.DTOs
{
    public class DirectedDTO
    {
        public int ID { get; set; }
        public SeriesDTO Series { get; set; }
        public PersonDTO Director { get; set; }
    }
}
