using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class GetAllEmployeeCafesQueryHandler : IRequestHandler<GetAllEmployeeCafesQuery, IEnumerable<EmployeeCafeDto>>
    {
        private readonly IEmployeeCafeResource employeeCafeResource;
        private readonly IMapper mapper;

        public GetAllEmployeeCafesQueryHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            this.employeeCafeResource = employeeCafeResource;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeCafeDto>> Handle(GetAllEmployeeCafesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EmployeeCafe> employeeCafes = await employeeCafeResource.GetAllAsync();
            
            return mapper.Map<IEnumerable<EmployeeCafeDto>>(employeeCafes);
        }
    }
} 