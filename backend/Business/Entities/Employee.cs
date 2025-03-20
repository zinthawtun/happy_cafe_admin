namespace Business.Entities
{
    public class Employee
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public string EmailAddress { get; private set; }

        public string Phone { get; private set; }

        public Gender Gender { get; private set; }

        public List<EmployeeCafe> EmployeeCafes { get; private set; } = new();

        public Employee(string id, string name, string emailAddress, string phone, Gender gender)
        {
            Id = id;
            Name = name;
            EmailAddress = emailAddress;
            Phone = phone;
            Gender = gender;
        }

        public void Update(string name, string emailAddress, string phone, Gender gender)
        {
            Name = name;
            EmailAddress = emailAddress;
            Phone = phone;
            Gender = gender;
        }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
