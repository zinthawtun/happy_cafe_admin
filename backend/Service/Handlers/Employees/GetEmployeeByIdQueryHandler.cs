using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Queries.Employees;

namespace Service.Handlers.Employees
{
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
    {
        private readonly IEmployeeResource employeeResource;
        private readonly IMapper mapper;

        public GetEmployeeByIdQueryHandler(IEmployeeResource employeeResource, IMapper mapper)
        {
            this.employeeResource = employeeResource;
            this.mapper = mapper;
        }

        public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            Employee? employee = await employeeResource.GetByIdAsync(request.Id);
            
            return employee != null ? mapper.Map<EmployeeDto>(employee) : null;
        }
    }
} 