namespace Business.Entities
{
    public class Cafe
    {
        public Guid ID { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Logo { get; private set; }

        public string Location { get; private set; }

        public Cafe(Guid id, string name, string description, string logo, string location)
        {
            ID = id;
            Name = name;
            Description = description;
            Logo = logo;
            Location = location;
        }

        public void Update(string name, string description, string logo, string location)
        {
            Name = name;
            Description = description;
            Logo = logo;
            Location = location;
        }
    }
}
