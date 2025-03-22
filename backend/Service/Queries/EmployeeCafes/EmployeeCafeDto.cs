using System;

namespace Service.Queries.EmployeeCafes
{
    public class EmployeeCafeDto
    {
        public Guid Id { get; set; }

        public Guid CafeId { get; set; }

        public string CafeName { get; set; } = string.Empty;

        public string EmployeeId { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }

        public bool IsActive { get; set; }
    }
} 