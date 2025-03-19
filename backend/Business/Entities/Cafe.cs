namespace Business.Entities
{
    public class Cafe
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Logo { get; private set; }

        public string Location { get; private set; }

        public List<EmployeeCafe> EmployeeCafes { get; private set; } = new();

        public Cafe(Guid id, string name, string description, string logo, string location)
        {
            Id = id;
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
