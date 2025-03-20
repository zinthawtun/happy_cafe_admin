using MediatR;

namespace Service.Commands.EmployeeCafes
{
    public class UnassignEmployeeFromCafeCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
} 