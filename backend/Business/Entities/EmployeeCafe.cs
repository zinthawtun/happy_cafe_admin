namespace Business.Entities
{
    public class EmployeeCafe
    {
        public Guid Id { get; private set; }

        public Guid CafeId { get; private set; }

        public string EmployeeId { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime AssignedDate { get; private set; }

        public Employee? Employee { get; private set; }

        public Cafe? Cafe { get; private set; }

        public EmployeeCafe(Guid id, Guid cafeId, string employeeId, DateTime assignedDate)
        {
            Id = id;
            CafeId = cafeId;
            EmployeeId = employeeId;
            IsActive = true;
            AssignedDate = assignedDate;
        }

        public void Update(Guid cafeId, string employeeId, bool isActive, DateTime assignedDate)
        {
            CafeId = cafeId;
            EmployeeId = employeeId;
            IsActive = isActive;
            AssignedDate = assignedDate;
        }
    }    
}
