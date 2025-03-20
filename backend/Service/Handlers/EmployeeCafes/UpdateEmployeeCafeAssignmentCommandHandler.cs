using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class UpdateEmployeeCafeAssignmentCommandHandler : IRequestHandler<UpdateEmployeeCafeAssignmentCommand, EmployeeCafe?>
    {
        private readonly IEmployeeCafeResource _employeeCafeResource;
        private readonly IMapper _mapper;

        public UpdateEmployeeCafeAssignmentCommandHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            _employeeCafeResource = employeeCafeResource;
            _mapper = mapper;
        }

        public async Task<EmployeeCafe?> Handle(UpdateEmployeeCafeAssignmentCommand request, CancellationToken cancellationToken)
        {
            return await _employeeCafeResource.UpdateAsync(
                request.Id,
                request.CafeId,
                request.EmployeeId,
                request.IsActive,
                request.AssignedDate);
        }
    }
} 