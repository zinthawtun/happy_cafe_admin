using Business.Entities;

namespace Service.Queries.Employees
{
    public class EmployeeDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public DateTime? AssignedDate { get; set; }
        
        public Guid? CafeId { get; set; }

        public string CafeName { get; set; } = string.Empty;

        public int DaysWorked { get; set; }

        public bool IsAssignedToCafe { get; set; }
    }
} 