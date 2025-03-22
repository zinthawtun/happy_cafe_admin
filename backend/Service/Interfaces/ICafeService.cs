using Service.Commands.Cafes;
using Service.Queries.Cafes;

namespace Service.Interfaces
{
    public interface ICafeService
    {
        Task<CafeDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<CafeDto>> GetByLocationAsync(string location);

        Task<IEnumerable<CafeDto>> GetAllAsync();
        
        Task<CafeDto?> CreateAsync(CreateCafeCommand command);

        Task<CafeDto?> UpdateAsync(UpdateCafeCommand command);

        Task<bool> DeleteAsync(Guid id);
    }
} 