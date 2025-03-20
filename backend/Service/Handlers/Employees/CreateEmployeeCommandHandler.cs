using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.Employees;

namespace Service.Handlers.Employees
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Employee>
    {
        private readonly IEmployeeResource employeeResource;
        private readonly IMapper mapper;

        public CreateEmployeeCommandHandler(IEmployeeResource employeeResource, IMapper mapper)
        {
            this.employeeResource = employeeResource;
            this.mapper = mapper;
        }

        public async Task<Employee> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await employeeResource.CreateAsync(
                request.Name,
                request.EmailAddress,
                request.Phone,
                request.Gender);
        }
    }
} 