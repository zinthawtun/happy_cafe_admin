using AutoMapper;
using MediatR;
using Resource.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class GetEmployeeCafesByEmployeeIdQueryHandler : IRequestHandler<GetEmployeeCafesByEmployeeIdQuery, IEnumerable<EmployeeCafeDto>>
    {
        private readonly IEmployeeCafeResource _employeeCafeResource;
        private readonly IMapper _mapper;

        public GetEmployeeCafesByEmployeeIdQueryHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            _employeeCafeResource = employeeCafeResource;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeCafeDto>> Handle(GetEmployeeCafesByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            var employeeCafes = await _employeeCafeResource.GetByEmployeeIdAsync(request.EmployeeId);
            
            return _mapper.Map<IEnumerable<EmployeeCafeDto>>(employeeCafes);
        }
    }
} 