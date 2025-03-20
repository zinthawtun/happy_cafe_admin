using Business.Entities;
using Utilities;

namespace Tests
{
    public class ScenarioHelper
    {
        public static Employee CreateEmployee(string name, string email, string phone, Gender gender)
        {
            return new Employee(
                UniqueIdGenerator.GenerateUniqueId(),
                name,
                email,
                phone,
                gender);
        }

        public static Cafe CreateCafe(string name, string description, string logo, string location)
        {
            return new Cafe(
                Guid.NewGuid(),
                name,
                description,
                logo,
                location);
        }
    }
}
