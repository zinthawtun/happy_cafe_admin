using Service.Commands.EmployeeCafes;
using Service.Queries.EmployeeCafes;

namespace Service.Interfaces
{
    public interface IEmployeeCafeService
    {
        Task<EmployeeCafeDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<EmployeeCafeDto>> GetAllAsync();

        Task<IEnumerable<EmployeeCafeDto>> GetByCafeIdAsync(Guid cafeId);

        Task<EmployeeCafeDto?> GetByEmployeeIdAsync(string employeeId);

        Task<EmployeeCafeDto?> AssignEmployeeToCafeAsync(AssignEmployeeToCafeCommand command);

        Task<EmployeeCafeDto?> UpdateAssignmentAsync(UpdateEmployeeCafeAssignmentCommand command);

        Task<bool> UnassignEmployeeFromCafeAsync(Guid id);

        Task<bool> IsEmployeeAssignedToCafeAsync(string employeeId, Guid cafeId);
    }
} 