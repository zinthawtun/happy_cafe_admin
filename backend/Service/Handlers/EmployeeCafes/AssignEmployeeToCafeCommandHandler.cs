using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class AssignEmployeeToCafeCommandHandler : IRequestHandler<AssignEmployeeToCafeCommand, EmployeeCafe>
    {
        private readonly IEmployeeCafeResource _employeeCafeResource;
        private readonly IMapper _mapper;

        public AssignEmployeeToCafeCommandHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            _employeeCafeResource = employeeCafeResource;
            _mapper = mapper;
        }

        public async Task<EmployeeCafe> Handle(AssignEmployeeToCafeCommand request, CancellationToken cancellationToken)
        {
            return await _employeeCafeResource.CreateAsync(
                request.CafeId,
                request.EmployeeId,
                request.AssignedDate);
        }
    }
} 