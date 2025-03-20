using MediatR;

namespace Service.Queries.Employees
{
    public class GetEmployeeByIdQuery : IRequest<EmployeeDto?>
    {
        public string Id { get; set; } = string.Empty;
    }
} 