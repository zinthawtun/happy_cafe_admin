using MediatR;

namespace Service.Queries.Employees
{
    public class GetEmployeesByCafeIdQuery : IRequest<IEnumerable<EmployeeDto>>
    {
        public Guid CafeId { get; set; }
    }
} 