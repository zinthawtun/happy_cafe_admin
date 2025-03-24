using Business.Entities;

namespace Resource.Interfaces
{
    public interface ICafeResource
    {
        Task<Cafe?> GetByIdAsync(Guid id);

        Task<IEnumerable<Cafe>> GetByLocationAsync(string location);

        Task<IEnumerable<Cafe>> GetAllAsync();

        Task<IEnumerable<Cafe>> GetByEmployeeIdAsync(string employeeId);
        
        Task<Cafe> CreateAsync(string name, string description, string logo, string location);
        
        Task<Cafe?> UpdateAsync(Guid id, string name, string description, string logo, string location);
        
        Task<bool> DeleteAsync(Guid id);
        
        Task<int> GetCountAsync();

        Task<bool> ExistsAsync(Guid id);
    }
} 