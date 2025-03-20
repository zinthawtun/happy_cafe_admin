using MediatR;

namespace Service.Commands.Employees
{
    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
    }
} 