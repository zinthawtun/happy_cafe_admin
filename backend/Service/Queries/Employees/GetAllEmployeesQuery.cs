using MediatR;

namespace Service.Queries.Employees
{
    public class GetAllEmployeesQuery : IRequest<IEnumerable<EmployeeDto>>
    {
    }
} 