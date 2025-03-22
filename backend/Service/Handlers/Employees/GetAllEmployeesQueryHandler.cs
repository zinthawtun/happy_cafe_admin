using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Queries.Employees;

namespace Service.Handlers.Employees
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>
    {
        private readonly IEmployeeResource employeeResource;
        private readonly IMapper mapper;

        public GetAllEmployeesQueryHandler(IEmployeeResource employeeResource, IMapper mapper)
        {
            this.employeeResource = employeeResource;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Employee> employees = await employeeResource.GetAllAsync();
            
            return mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }
    }
} 