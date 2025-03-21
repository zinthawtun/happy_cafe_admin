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

        public async Task<Employee?> GetByIdAsync(string id)
        {
            GetEmployeeByIdQuery query = new GetEmployeeByIdQuery { Id = id };

            EmployeeDto? employeeDto = await mediator.Send(query);
            
            if (employeeDto == null)
                return null;
                
            return mapper.Map<Employee>(employeeDto);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            GetAllEmployeesQuery query = new GetAllEmployeesQuery();

            IEnumerable<EmployeeDto> employeeDtos = await mediator.Send(query);
            
            return mapper.Map<IEnumerable<Employee>>(employeeDtos);
        }

        public async Task<IEnumerable<Employee>> GetByCafeIdAsync(Guid cafeId)
        {
            GetEmployeesByCafeIdQuery query = new GetEmployeesByCafeIdQuery { CafeId = cafeId };

            IEnumerable<EmployeeDto> employeeDtos = await mediator.Send(query);
            
            return mapper.Map<IEnumerable<Employee>>(employeeDtos);
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand
            {
                Name = employee.Name,
                EmailAddress = employee.EmailAddress,
                Phone = employee.Phone,
                Gender = employee.Gender
            };
            
            return await mediator.Send(command);
        }

        public async Task<Employee?> UpdateAsync(Employee employee)
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand
            {
                Id = employee.Id,
                Name = employee.Name,
                EmailAddress = employee.EmailAddress,
                Phone = employee.Phone,
                Gender = employee.Gender
            };
            
            return await mediator.Send(command);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            DeleteEmployeeCommand command = new DeleteEmployeeCommand { Id = id };

            return await mediator.Send(command);
        }
    }
} 