using Business.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resource.Interfaces
{
    public interface IEmployeeCafeResource
    {
        Task<EmployeeCafe?> GetByIdAsync(Guid id);

        Task<IEnumerable<EmployeeCafe>> GetAllAsync();

        Task<IEnumerable<EmployeeCafe>> GetByCafeIdAsync(Guid cafeId);

        Task<IEnumerable<EmployeeCafe>> GetByEmployeeIdAsync(string employeeId);
        
        Task<EmployeeCafe> CreateAsync(Guid cafeId, string employeeId, DateTime assignedDate);
        
        Task<EmployeeCafe?> UpdateAsync(Guid id, Guid cafeId, string employeeId, bool isActive, DateTime assignedDate);
        
        Task<bool> DeleteAsync(Guid id);
        
        Task<bool> ExistsAsync(Guid id);

        Task<bool> IsEmployeeAssignedToCafeAsync(string employeeId, Guid cafeId);
    }
} 