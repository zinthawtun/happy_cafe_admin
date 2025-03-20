using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.Employees;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Handlers.Employees
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Employee?>
    {
        private readonly IEmployeeResource employeeResource;
        private readonly IMapper mapper;

        public UpdateEmployeeCommandHandler(IEmployeeResource employeeResource, IMapper mapper)
        {
            this.employeeResource = employeeResource;
            this.mapper = mapper;
        }

        public async Task<Employee?> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await employeeResource.UpdateAsync(
                request.Id,
                request.Name,
                request.EmailAddress,
                request.Phone,
                request.Gender);
        }
    }
} 