using Business.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resource.Interfaces
{
    public interface IEmployeeResource
    {
        Task<Employee?> GetByIdAsync(string id);

        Task<IEnumerable<Employee>> GetAllAsync();

        Task<IEnumerable<Employee>> GetByCafeIdAsync(Guid cafeId);
        
        Task<Employee> CreateAsync(string name, string emailAddress, string phone, Gender gender);
        
        Task<Employee?> UpdateAsync(string id, string name, string emailAddress, string phone, Gender gender);
        
        Task<bool> DeleteAsync(string id);
        
        Task<int> GetCountAsync();
        Task<bool> ExistsAsync(string id);
    }
} 