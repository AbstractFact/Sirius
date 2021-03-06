namespace Sirius.DTOs
{
    public class RoleDTO
    {
        public int ID { get; set; }
        public SeriesDTO Series { get; set; }
        public PersonDTO Actor { get; set; }
        public string InRole { get; set; }
    }
}
