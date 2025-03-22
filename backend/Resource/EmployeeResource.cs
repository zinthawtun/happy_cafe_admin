using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Resource.Interfaces;
using Utilities;

namespace Resource
{
    public class EmployeeResource : IEmployeeResource
    {
        private readonly AppDbContext dbContext;

        public EmployeeResource(AppDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Employee?> GetByIdAsync(string id)
        {
            return await dbContext.Employees
                .Include(e => e.EmployeeCafes!)
                .ThenInclude(ec => ec.Cafe!)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await dbContext.Employees
                .Include(e => e.EmployeeCafes!)
                .ThenInclude(ec => ec.Cafe!)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByCafeIdAsync(Guid cafeId)
        {
            return await dbContext.EmployeeCafes
                .Where(ec => ec.CafeId == cafeId && ec.IsActive)
                .Include(ec => ec.Employee!)
                .ThenInclude(e => e.EmployeeCafes!)
                .ThenInclude(ec => ec.Cafe!)
                .Select(ec => ec.Employee!)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Employee> CreateAsync(string name, string emailAddress, string phone, Gender gender)
        {
            string id;
            do
            {
                id = UniqueIdGenerator.GenerateUniqueId();
            } while (await dbContext.Employees.AnyAsync(e => e.Id == id));

            Employee employee = new Employee(id, name, emailAddress, phone, gender);

            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
            
            return employee;
        }

        public async Task<Employee?> UpdateAsync(string id, string name, string emailAddress, string phone, Gender gender)
        {
            Employee? employee = await dbContext.Employees.FindAsync(id);

            if (employee == null)
                return null;

            employee.Update(name, emailAddress, phone, gender);
            dbContext.Employees.Update(employee);
            await dbContext.SaveChangesAsync();
            
            return employee;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Employee? employee = await dbContext.Employees.FindAsync(id);

            if (employee == null)
                return false;

            bool hasActiveCafes = await dbContext.EmployeeCafes
                .AnyAsync(ec => ec.EmployeeId == id && ec.IsActive);
                
            if (hasActiveCafes)
                return false;

            dbContext.Employees.Remove(employee);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCountAsync()
        {
            return await dbContext.Employees.CountAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await dbContext.Employees.AnyAsync(e => e.Id == id);
        }
    }
} 