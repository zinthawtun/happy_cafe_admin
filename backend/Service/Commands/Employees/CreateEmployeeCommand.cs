using Business.Entities;
using MediatR;

namespace Service.Commands.Employees
{
    public class CreateEmployeeCommand : IRequest<Employee>
    {
        public string Name { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Gender Gender { get; set; }
    }
} 