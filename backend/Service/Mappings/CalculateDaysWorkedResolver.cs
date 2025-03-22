using AutoMapper;
using Business.Entities;
using Service.Queries.Employees;

namespace Service.Mappings
{
    public class CalculateDaysWorkedResolver : IValueResolver<Employee, EmployeeDto, int>
    {
        public int Resolve(Employee source, EmployeeDto destination, int destMember, ResolutionContext context)
        {
            if (source.EmployeeCafes == null || !source.EmployeeCafes.Any(ec => ec.IsActive))
                return 0;

            EmployeeCafe? activeEmployeeCafe = source.EmployeeCafes.FirstOrDefault(ec => ec.IsActive);

            if (activeEmployeeCafe == null)
                return 0;

            return (int)Math.Ceiling((DateTime.UtcNow - activeEmployeeCafe.AssignedDate).TotalDays);
        }
    }
}
