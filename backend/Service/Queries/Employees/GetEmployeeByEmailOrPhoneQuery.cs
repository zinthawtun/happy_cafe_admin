using MediatR;

namespace Service.Queries.Employees
{
    public class GetEmployeeByEmailOrPhoneQuery : IRequest<EmployeeDto?>
    {
        public string EmailAddress { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
} 