using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class AssignEmployeeToCafeCommandHandler : IRequestHandler<AssignEmployeeToCafeCommand, EmployeeCafe>
    {
        private readonly IEmployeeCafeResource employeeCafeResource;
        private readonly IMapper mapper;

        public AssignEmployeeToCafeCommandHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            this.employeeCafeResource = employeeCafeResource;
            this.mapper = mapper;
        }

        public async Task<EmployeeCafe> Handle(AssignEmployeeToCafeCommand request, CancellationToken cancellationToken)
        {
            return await employeeCafeResource.CreateAsync(
                request.CafeId,
                request.EmployeeId,
                request.AssignedDate);
        }
    }
} 