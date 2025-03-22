using AutoMapper;
using Business.Entities;
using Service.Queries.Employees;

namespace Service.Mappings
{
    public class EmployeeCafeNameResolver : IValueResolver<Employee, EmployeeDto, string>
    {
        public string Resolve(Employee source, EmployeeDto destination, string destMember, ResolutionContext context)
        {
            if (source.EmployeeCafes == null || !source.EmployeeCafes.Any(ec => ec.IsActive))
                return string.Empty;

            EmployeeCafe? activeEmployeeCafe = source.EmployeeCafes.FirstOrDefault(ec => ec.IsActive);

            if (activeEmployeeCafe == null || activeEmployeeCafe.Cafe == null)
                return string.Empty;

            return activeEmployeeCafe.Cafe.Name;
        }
    }
}
