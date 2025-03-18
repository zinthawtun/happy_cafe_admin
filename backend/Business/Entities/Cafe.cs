using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entities
{
    public class Cafe
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Logo { get; private set; }

        public string Location { get; private set; }

        public Cafe(string name, string description, string logo, string location)
        {
            Id = Guid.NewGuid();
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
