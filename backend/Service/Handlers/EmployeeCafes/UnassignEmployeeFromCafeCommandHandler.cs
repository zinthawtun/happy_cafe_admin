using MediatR;
using Resource.Interfaces;
using Service.Commands.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class UnassignEmployeeFromCafeCommandHandler : IRequestHandler<UnassignEmployeeFromCafeCommand, bool>
    {
        private readonly IEmployeeCafeResource employeeCafeResource;

        public UnassignEmployeeFromCafeCommandHandler(IEmployeeCafeResource employeeCafeResource)
        {
            this.employeeCafeResource = employeeCafeResource;
        }

        public async Task<bool> Handle(UnassignEmployeeFromCafeCommand request, CancellationToken cancellationToken)
        {
            return await employeeCafeResource.DeleteAsync(request.Id);
        }
    }
} 