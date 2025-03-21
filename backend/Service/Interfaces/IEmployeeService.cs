using Business.Entities;

namespace Service.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee?> GetByIdAsync(string id);

        Task<IEnumerable<Employee>> GetAllAsync();

        Task<IEnumerable<Employee>> GetByCafeIdAsync(Guid cafeId);
        
        Task<Employee> CreateAsync(Employee employee);

        Task<Employee?> UpdateAsync(Employee employee);

        Task<bool> DeleteAsync(string id);
    }
} 