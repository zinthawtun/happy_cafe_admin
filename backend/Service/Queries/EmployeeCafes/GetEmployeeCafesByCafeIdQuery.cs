using MediatR;
using System;
using System.Collections.Generic;

namespace Service.Queries.EmployeeCafes
{
    public class GetEmployeeCafesByCafeIdQuery : IRequest<IEnumerable<EmployeeCafeDto>>
    {
        public Guid CafeId { get; set; }
    }
} 