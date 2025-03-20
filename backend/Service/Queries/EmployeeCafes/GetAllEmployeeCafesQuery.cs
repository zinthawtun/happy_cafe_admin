using MediatR;
using System.Collections.Generic;

namespace Service.Queries.EmployeeCafes
{
    public class GetAllEmployeeCafesQuery : IRequest<IEnumerable<EmployeeCafeDto>>
    {
    }
} 