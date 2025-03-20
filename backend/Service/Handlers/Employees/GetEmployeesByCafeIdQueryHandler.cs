using AutoMapper;
using MediatR;
using Resource.Interfaces;
using Service.Queries.Employees;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Handlers.Employees
{
    public class GetEmployeesByCafeIdQueryHandler : IRequestHandler<GetEmployeesByCafeIdQuery, IEnumerable<EmployeeDto>>
    {
        private readonly IEmployeeResource employeeResource;
        private readonly IMapper mapper;

        public GetEmployeesByCafeIdQueryHandler(IEmployeeResource employeeResource, IMapper mapper)
        {
            this.employeeResource = employeeResource;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeesByCafeIdQuery request, CancellationToken cancellationToken)
        {
            var employees = await employeeResource.GetByCafeIdAsync(request.CafeId);
            
            return mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }
    }
} 