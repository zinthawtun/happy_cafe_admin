using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class GetEmployeeCafeByIdQueryHandler : IRequestHandler<GetEmployeeCafeByIdQuery, EmployeeCafeDto?>
    {
        private readonly IEmployeeCafeResource employeeCafeResource;
        private readonly IMapper mapper;

        public GetEmployeeCafeByIdQueryHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            this.employeeCafeResource = employeeCafeResource;
            this.mapper = mapper;
        }

        public async Task<EmployeeCafeDto?> Handle(GetEmployeeCafeByIdQuery request, CancellationToken cancellationToken)
        {
            EmployeeCafe? employeeCafe = await employeeCafeResource.GetByIdAsync(request.Id);
            
            return employeeCafe != null ? mapper.Map<EmployeeCafeDto>(employeeCafe) : null;
        }
    }
} 