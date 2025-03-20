using MediatR;
using Resource.Interfaces;
using Service.Commands.Employees;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Handlers.Employees
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
    {
        private readonly IEmployeeResource employeeResource;

        public DeleteEmployeeCommandHandler(IEmployeeResource employeeResource)
        {
            this.employeeResource = employeeResource;
        }

        public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await employeeResource.DeleteAsync(request.Id);
        }
    }
} 