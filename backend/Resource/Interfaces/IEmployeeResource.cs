using Business.Entities;

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

        Task<Employee?> FindByEmailOrPhoneAsync(string emailAddress, string phone);
    }
} 