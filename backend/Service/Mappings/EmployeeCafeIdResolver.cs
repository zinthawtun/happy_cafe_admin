using AutoMapper;
using Business.Entities;
using Service.Queries.Employees;

namespace Service.Mappings
{
    public class EmployeeCafeIdResolver : IValueResolver<Employee, EmployeeDto, Guid?>
    {
        public Guid? Resolve(Employee source, EmployeeDto destination, Guid? destMember, ResolutionContext context)
        {
            if (source.EmployeeCafes == null || !source.EmployeeCafes.Any(ec => ec.IsActive))
                return null;

            EmployeeCafe? activeEmployeeCafe = source.EmployeeCafes.FirstOrDefault(ec => ec.IsActive);

            return activeEmployeeCafe != null ? activeEmployeeCafe.CafeId : null;
        }
    }
}
