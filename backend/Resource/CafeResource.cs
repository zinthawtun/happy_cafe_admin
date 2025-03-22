using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Resource.Interfaces;

namespace Resource
{
    public class CafeResource : ICafeResource
    {
        private readonly AppDbContext dbContext;

        public CafeResource(AppDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Cafe?> GetByIdAsync(Guid id)
        {
            return await dbContext.Cafes
                .Include(c => c.EmployeeCafes)
                .ThenInclude(ec => ec.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cafe>> GetByLocationAsync(string location)
        {
            return await dbContext.Cafes
                .Where(c => EF.Functions.Like(c.Location.ToLower(), $"%{location.ToLower()}%"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Cafe>> GetAllAsync()
        {
            return await dbContext.Cafes
                .Include(c => c.EmployeeCafes)
                .ThenInclude(ec => ec.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cafe>> GetByEmployeeIdAsync(string employeeId)
        {
            return await dbContext.EmployeeCafes
                .Where(ec => ec.EmployeeId == employeeId && ec.IsActive)
                .Include(ec => ec.Cafe)
                .Select(ec => ec.Cafe!)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Cafe> CreateAsync(string name, string description, string logo, string location)
        {
            Cafe cafe = new Cafe(Guid.NewGuid(), name, description, logo, location);

            await dbContext.Cafes.AddAsync(cafe);

            await dbContext.SaveChangesAsync();
            
            return cafe;
        }

        public async Task<Cafe?> UpdateAsync(Guid id, string name, string description, string logo, string location)
        {
            Cafe? cafe = await dbContext.Cafes.FindAsync(id);

            if (cafe == null)
                return null;

            cafe.Update(name, description, logo, location);

            dbContext.Cafes.Update(cafe);

            await dbContext.SaveChangesAsync();
            
            return cafe;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Cafe? cafe = await dbContext.Cafes.FindAsync(id);

            if (cafe == null)
                return false;

            
            bool hasActiveEmployees = await dbContext.EmployeeCafes
                .AnyAsync(ec => ec.CafeId == id && ec.IsActive);
                
            if (hasActiveEmployees)
                return false;

            dbContext.Cafes.Remove(cafe);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCountAsync()
        {
            return await dbContext.Cafes.CountAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await dbContext.Cafes.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await dbContext.Cafes.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }
} 