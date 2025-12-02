namespace DogsHouse.Db.Entities
{
    public class Dog : Entity
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public double TailLength { get; set; }
        public double Weight { get; set; }
    }
}
