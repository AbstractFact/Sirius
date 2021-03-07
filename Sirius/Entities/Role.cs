namespace Sirius.Entities
{
    public class Role
    {
        public Series Series { get; set; }
        public Person Actor { get; set; }
        public string InRole { get; set; }
    }
}
