using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;

namespace Business.Entities
{
    public class Employee
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public string EmailAddress { get; private set; }

        public string Phone { get; private set; }

        public string Gender { get; private set; }

        public Employee(string name, string emailAddress, string phone, string gender)
        {
            Id = UniqueIdGenerator.GenerateUniqueId();
            Name = name;
            EmailAddress = emailAddress;
            Phone = phone;
            Gender = gender;
        }

        public void Update(string name, string emailAddress, string phone, string gender)
        {
            Name = name;
            EmailAddress = emailAddress;
            Phone = phone;
            Gender = gender;
        }
    }
}
