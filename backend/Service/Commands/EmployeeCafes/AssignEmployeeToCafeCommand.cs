using Business.Entities;
using MediatR;

namespace Service.Commands.EmployeeCafes
{
    public class AssignEmployeeToCafeCommand : IRequest<EmployeeCafe>
    {
        public Guid CafeId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
} 