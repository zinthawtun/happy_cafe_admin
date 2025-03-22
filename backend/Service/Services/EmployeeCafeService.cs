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

        public async Task<EmployeeCafeDto?> GetByIdAsync(Guid id)
        {
            GetEmployeeCafeByIdQuery query = new GetEmployeeCafeByIdQuery { Id = id };

            return await mediator.Send(query);
        }

        public async Task<IEnumerable<EmployeeCafeDto>> GetAllAsync()
        {
            GetAllEmployeeCafesQuery query = new GetAllEmployeeCafesQuery();

            return await mediator.Send(query);
        }

        public async Task<IEnumerable<EmployeeCafeDto>> GetByCafeIdAsync(Guid cafeId)
        {
            GetEmployeeCafesByCafeIdQuery query = new GetEmployeeCafesByCafeIdQuery { CafeId = cafeId };

            return await mediator.Send(query);
        }

        public async Task<EmployeeCafeDto?> GetByEmployeeIdAsync(string employeeId)
        {
            GetEmployeeCafesByEmployeeIdQuery query = new GetEmployeeCafesByEmployeeIdQuery { EmployeeId = employeeId };

            IEnumerable<EmployeeCafeDto> employeeCafeDtos = await mediator.Send(query);

            return employeeCafeDtos.FirstOrDefault(ec => ec.IsActive);
        }

        public async Task<EmployeeCafeDto?> AssignEmployeeToCafeAsync(AssignEmployeeToCafeCommand command)
        {
            EmployeeCafe employeeCafe = await mediator.Send(command);

            return mapper.Map<EmployeeCafeDto>(employeeCafe);
        }

        public async Task<EmployeeCafeDto?> UpdateAssignmentAsync(UpdateEmployeeCafeAssignmentCommand command)
        {
            EmployeeCafe? employeeCafe = await mediator.Send(command);

            if (employeeCafe == null)
                return null;

            return mapper.Map<EmployeeCafeDto>(employeeCafe);
        }

        public async Task<bool> UnassignEmployeeFromCafeAsync(Guid id)
        {
            UnassignEmployeeFromCafeCommand command = new UnassignEmployeeFromCafeCommand { Id = id };

            return await mediator.Send(command);
        }

        public async Task<bool> IsEmployeeAssignedToCafeAsync(string employeeId, Guid cafeId)
        {
            EmployeeCafeDto? assignment = await GetByEmployeeIdAsync(employeeId);

            return assignment != null && assignment.CafeId == cafeId && assignment.IsActive;
        }
    }
} 