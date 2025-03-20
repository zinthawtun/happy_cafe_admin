using Business.Entities;

namespace Service.Interfaces
{
    public interface ICafeService
    {
        Task<Cafe?> GetByIdAsync(Guid id);
        Task<IEnumerable<Cafe>> GetByLocationAsync(string location);
        Task<IEnumerable<Cafe>> GetAllAsync();
        
        Task<Cafe> CreateAsync(Cafe cafe);
        Task<Cafe?> UpdateAsync(Cafe cafe);
        Task<bool> DeleteAsync(Guid id);
    }
} 