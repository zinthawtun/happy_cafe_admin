using MediatR;
using Resource.Interfaces;
using Service.Commands.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class UnassignEmployeeFromCafeCommandHandler : IRequestHandler<UnassignEmployeeFromCafeCommand, bool>
    {
        private readonly IEmployeeCafeResource _employeeCafeResource;

        public UnassignEmployeeFromCafeCommandHandler(IEmployeeCafeResource employeeCafeResource)
        {
            _employeeCafeResource = employeeCafeResource;
        }

        public async Task<bool> Handle(UnassignEmployeeFromCafeCommand request, CancellationToken cancellationToken)
        {
            return await _employeeCafeResource.DeleteAsync(request.Id);
        }
    }
} 