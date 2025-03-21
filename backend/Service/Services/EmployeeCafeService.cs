using AutoMapper;
using Business.Entities;
using MediatR;
using Service.Commands.EmployeeCafes;
using Service.Interfaces;
using Service.Queries.EmployeeCafes;

namespace Service.Services
{
    public class EmployeeCafeService : IEmployeeCafeService
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public EmployeeCafeService(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task<EmployeeCafe?> GetByIdAsync(Guid id)
        {
            GetEmployeeCafeByIdQuery query = new GetEmployeeCafeByIdQuery { Id = id };

            EmployeeCafeDto? employeeCafeDto = await mediator.Send(query);
            
            if (employeeCafeDto == null)
                return null;
                
            return mapper.Map<EmployeeCafe>(employeeCafeDto);
        }

        public async Task<IEnumerable<EmployeeCafe>> GetAllAsync()
        {
            GetAllEmployeeCafesQuery query = new GetAllEmployeeCafesQuery();

            IEnumerable<EmployeeCafeDto> employeeCafeDtos = await mediator.Send(query);
            
            return mapper.Map<IEnumerable<EmployeeCafe>>(employeeCafeDtos);
        }

        public async Task<IEnumerable<EmployeeCafe>> GetByCafeIdAsync(Guid cafeId)
        {
            GetEmployeeCafesByCafeIdQuery query = new GetEmployeeCafesByCafeIdQuery { CafeId = cafeId };

            IEnumerable<EmployeeCafeDto> employeeCafeDtos = await mediator.Send(query);
            
            return mapper.Map<IEnumerable<EmployeeCafe>>(employeeCafeDtos);
        }

        public async Task<IEnumerable<EmployeeCafe>> GetByEmployeeIdAsync(string employeeId)
        {
            GetEmployeeCafesByEmployeeIdQuery query = new GetEmployeeCafesByEmployeeIdQuery { EmployeeId = employeeId };

            IEnumerable<EmployeeCafeDto> employeeCafeDtos = await mediator.Send(query);
            
            return mapper.Map<IEnumerable<EmployeeCafe>>(employeeCafeDtos);
        }

        public async Task<EmployeeCafe> AssignEmployeeToCafeAsync(Guid cafeId, string employeeId, DateTime assignedDate)
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand
            {
                CafeId = cafeId,
                EmployeeId = employeeId,
                AssignedDate = assignedDate
            };
            
            return await mediator.Send(command);
        }

        public async Task<EmployeeCafe?> UpdateAssignmentAsync(Guid id, Guid cafeId, string employeeId, bool isActive, DateTime assignedDate)
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand
            {
                Id = id,
                CafeId = cafeId,
                EmployeeId = employeeId,
                IsActive = isActive,
                AssignedDate = assignedDate
            };
            
            return await mediator.Send(command);
        }

        public async Task<bool> UnassignEmployeeFromCafeAsync(Guid id)
        {
            UnassignEmployeeFromCafeCommand command = new UnassignEmployeeFromCafeCommand { Id = id };

            return await mediator.Send(command);
        }

        public async Task<bool> IsEmployeeAssignedToCafeAsync(string employeeId, Guid cafeId)
        {
            IEnumerable<EmployeeCafe> assignments = await GetByEmployeeIdAsync(employeeId);

            return assignments.Any(a => a.CafeId == cafeId && a.IsActive);
        }
    }
} 