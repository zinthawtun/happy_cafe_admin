using AutoMapper;
using MediatR;
using Resource.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class GetEmployeeCafeByIdQueryHandler : IRequestHandler<GetEmployeeCafeByIdQuery, EmployeeCafeDto?>
    {
        private readonly IEmployeeCafeResource _employeeCafeResource;
        private readonly IMapper _mapper;

        public GetEmployeeCafeByIdQueryHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            _employeeCafeResource = employeeCafeResource;
            _mapper = mapper;
        }

        public async Task<EmployeeCafeDto?> Handle(GetEmployeeCafeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeCafe = await _employeeCafeResource.GetByIdAsync(request.Id);
            
            return employeeCafe != null ? _mapper.Map<EmployeeCafeDto>(employeeCafe) : null;
        }
    }
} 