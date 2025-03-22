using Business.Entities;
using Utilities;
using System.Text.RegularExpressions;

namespace Tests
{
    public class ScenarioHelper
    {
        public static Employee CreateEmployee(string name, string email, string phone, Gender gender)
        {
            string validPhone = phone;
            if (!Regex.IsMatch(phone, @"^[89]\d{7}$"))
            {
                validPhone = "8" + new string('0', 7);
            }
            
            return new Employee(
                UniqueIdGenerator.GenerateUniqueId(),
                name,
                email,
                validPhone,
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
