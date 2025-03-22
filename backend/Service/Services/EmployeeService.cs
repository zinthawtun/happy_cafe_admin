using AutoMapper;
using Business.Entities;
using MediatR;
using Service.Commands.Employees;
using Service.Interfaces;
using Service.Queries.Employees;

namespace Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public EmployeeService(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task<EmployeeDto?> GetByIdAsync(string id)
        {
            GetEmployeeByIdQuery query = new GetEmployeeByIdQuery { Id = id };

            return await mediator.Send(query);
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            GetAllEmployeesQuery query = new GetAllEmployeesQuery();

            return await mediator.Send(query);
        }

        public async Task<IEnumerable<EmployeeDto>> GetByCafeIdAsync(Guid cafeId)
        {
            GetEmployeesByCafeIdQuery query = new GetEmployeesByCafeIdQuery { CafeId = cafeId };

            return await mediator.Send(query);
        }

        public async Task<EmployeeDto?> CreateAsync(CreateEmployeeCommand command)
        {
            Employee employee = await mediator.Send(command);

            return mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto?> UpdateAsync(UpdateEmployeeCommand command)
        {
            Employee? employee = await mediator.Send(command);

            if (employee == null)
                return null;

            return mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            DeleteEmployeeCommand command = new DeleteEmployeeCommand { Id = id };

            return await mediator.Send(command);
        }
    }
} 