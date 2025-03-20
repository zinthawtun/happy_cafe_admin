using MediatR;
using System;

namespace Service.Queries.EmployeeCafes
{
    public class GetEmployeeCafeByIdQuery : IRequest<EmployeeCafeDto?>
    {
        public Guid Id { get; set; }
    }
} 