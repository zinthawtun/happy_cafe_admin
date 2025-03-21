using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.EmployeeCafes;

namespace Service.Handlers.EmployeeCafes
{
    public class UpdateEmployeeCafeAssignmentCommandHandler : IRequestHandler<UpdateEmployeeCafeAssignmentCommand, EmployeeCafe?>
    {
        private readonly IEmployeeCafeResource employeeCafeResource;
        private readonly IMapper mapper;

        public UpdateEmployeeCafeAssignmentCommandHandler(IEmployeeCafeResource employeeCafeResource, IMapper mapper)
        {
            this.employeeCafeResource = employeeCafeResource;
            this.mapper = mapper;
        }

        public async Task<EmployeeCafe?> Handle(UpdateEmployeeCafeAssignmentCommand request, CancellationToken cancellationToken)
        {
            return await employeeCafeResource.UpdateAsync(
                request.Id,
                request.CafeId,
                request.EmployeeId,
                request.IsActive,
                request.AssignedDate);
        }
    }
} 