using MediatR;
using System.Collections.Generic;

namespace Service.Queries.EmployeeCafes
{
    public class GetEmployeeCafesByEmployeeIdQuery : IRequest<IEnumerable<EmployeeCafeDto>>
    {
        public string EmployeeId { get; set; } = string.Empty;
    }
} 