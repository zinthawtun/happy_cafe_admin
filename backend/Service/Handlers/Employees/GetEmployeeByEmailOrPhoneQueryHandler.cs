using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Queries.Employees;

namespace Service.Handlers.Employees
{
    public class GetEmployeeByEmailOrPhoneQueryHandler : IRequestHandler<GetEmployeeByEmailOrPhoneQuery, EmployeeDto?>
    {
        private readonly IEmployeeResource employeeResource;
        private readonly IMapper mapper;

        public GetEmployeeByEmailOrPhoneQueryHandler(IEmployeeResource employeeResource, IMapper mapper)
        {
            this.employeeResource = employeeResource;
            this.mapper = mapper;
        }

        public async Task<EmployeeDto?> Handle(GetEmployeeByEmailOrPhoneQuery request, CancellationToken cancellationToken)
        {
            Employee? employee = await employeeResource.FindByEmailOrPhoneAsync(request.EmailAddress, request.Phone);
            
            if (employee == null)
                return null;
                
            return mapper.Map<EmployeeDto>(employee);
        }
    }
} 