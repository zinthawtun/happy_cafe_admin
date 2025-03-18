namespace Business.Entities
{
    public class Employee
    {
        public string ID { get; private set; }

        public string Name { get; private set; }

        public string EmailAddress { get; private set; }

        public string Phone { get; private set; }

        public string Gender { get; private set; }

        public Employee(string id, string name, string emailAddress, string phone, string gender)
        {
            ID = id;
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
