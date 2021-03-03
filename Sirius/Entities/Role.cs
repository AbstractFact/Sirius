namespace Sirius.Entities
{
    public class Role
    {
        public int ID { get; set; }
        public Series Series { get; set; }
        public Person Actor { get; set; }
        public string InRole { get; set; }
    }
}
