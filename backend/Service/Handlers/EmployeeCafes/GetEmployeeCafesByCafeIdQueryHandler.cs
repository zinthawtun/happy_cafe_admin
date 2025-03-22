using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class GetEmployeeCafesByCafeIdQueryHandler : IRequestHandler<GetEmployeeCafesByCafeIdQuery, IEnumerable<EmployeeCafeDto>>
    {
        private readonly IEmployeeCafeResource employeeCafeResource;
        private readonly IMapper mapper;

        public GetEmployeeCafesByCafeIdQueryHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            this.employeeCafeResource = employeeCafeResource;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeCafeDto>> Handle(GetEmployeeCafesByCafeIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EmployeeCafe> employeeCafes = await employeeCafeResource.GetByCafeIdAsync(request.CafeId);
            
            return mapper.Map<IEnumerable<EmployeeCafeDto>>(employeeCafes);
        }
    }
} 