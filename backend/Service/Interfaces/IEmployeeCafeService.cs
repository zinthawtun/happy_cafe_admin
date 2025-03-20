using Business.Entities;

namespace Service.Interfaces
{
    public interface IEmployeeCafeService
    {
        Task<EmployeeCafe?> GetByIdAsync(Guid id);
        Task<IEnumerable<EmployeeCafe>> GetAllAsync();
        Task<IEnumerable<EmployeeCafe>> GetByCafeIdAsync(Guid cafeId);
        Task<IEnumerable<EmployeeCafe>> GetByEmployeeIdAsync(string employeeId);
        

        Task<EmployeeCafe> AssignEmployeeToCafeAsync(Guid cafeId, string employeeId, DateTime assignedDate);
        Task<EmployeeCafe?> UpdateAssignmentAsync(Guid id, Guid cafeId, string employeeId, bool isActive, DateTime assignedDate);
        Task<bool> UnassignEmployeeFromCafeAsync(Guid id);
        
        Task<bool> IsEmployeeAssignedToCafeAsync(string employeeId, Guid cafeId);
    }
} 