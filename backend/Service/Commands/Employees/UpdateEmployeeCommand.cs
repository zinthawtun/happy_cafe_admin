using Business.Entities;
using MediatR;

namespace Service.Commands.Employees
{
    public class UpdateEmployeeCommand : IRequest<Employee?>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Gender Gender { get; set; }
    }
} 