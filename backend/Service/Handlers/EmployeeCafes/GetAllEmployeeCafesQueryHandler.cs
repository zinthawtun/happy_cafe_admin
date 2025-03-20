using AutoMapper;
using MediatR;
using Resource.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class GetAllEmployeeCafesQueryHandler : IRequestHandler<GetAllEmployeeCafesQuery, IEnumerable<EmployeeCafeDto>>
    {
        private readonly IEmployeeCafeResource _employeeCafeResource;
        private readonly IMapper _mapper;

        public GetAllEmployeeCafesQueryHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            _employeeCafeResource = employeeCafeResource;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeCafeDto>> Handle(GetAllEmployeeCafesQuery request, CancellationToken cancellationToken)
        {
            var employeeCafes = await _employeeCafeResource.GetAllAsync();
            
            return _mapper.Map<IEnumerable<EmployeeCafeDto>>(employeeCafes);
        }
    }
} 