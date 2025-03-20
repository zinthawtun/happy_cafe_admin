using Business.Entities;
using MediatR;

namespace Service.Commands.EmployeeCafes
{
    public class UpdateEmployeeCafeAssignmentCommand : IRequest<EmployeeCafe?>
    {
        public Guid Id { get; set; }
        public Guid CafeId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime AssignedDate { get; set; }
    }
} 