using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class GetEmployeeCafesByEmployeeIdQueryHandler : IRequestHandler<GetEmployeeCafesByEmployeeIdQuery, IEnumerable<EmployeeCafeDto>>
    {
        private readonly IEmployeeCafeResource employeeCafeResource;
        private readonly IMapper mapper;

        public GetEmployeeCafesByEmployeeIdQueryHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            this.employeeCafeResource = employeeCafeResource;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeCafeDto>> Handle(GetEmployeeCafesByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EmployeeCafe> employeeCafes = await employeeCafeResource.GetByEmployeeIdAsync(request.EmployeeId);
            
            return mapper.Map<IEnumerable<EmployeeCafeDto>>(employeeCafes);
        }
    }
} 