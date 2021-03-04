namespace Sirius.Entities
{
    public class Directed
    {
        public int ID { get; set; }
        public Series Series { get; set; }
        public Person Director { get; set; }
    }
}
