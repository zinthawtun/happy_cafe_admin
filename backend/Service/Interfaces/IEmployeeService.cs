using Service.Commands.Employees;
using Service.Queries.Employees;

namespace Service.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetByIdAsync(string id);

        Task<IEnumerable<EmployeeDto>> GetAllAsync();

        Task<IEnumerable<EmployeeDto>> GetByCafeIdAsync(Guid cafeId);
        
        Task<EmployeeDto?> CreateAsync(CreateEmployeeCommand command);

        Task<EmployeeDto?> UpdateAsync(UpdateEmployeeCommand command);

        Task<bool> DeleteAsync(string id);

        Task<bool> ExistsWithEmailOrPhoneAsync(string emailAddress, string phone);
    }
} 