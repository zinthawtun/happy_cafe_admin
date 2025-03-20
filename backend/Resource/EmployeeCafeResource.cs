using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Resource.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resource
{
    public class EmployeeCafeResource : IEmployeeCafeResource
    {
        private readonly AppDbContext dbContext;

        public EmployeeCafeResource(AppDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<EmployeeCafe?> GetByIdAsync(Guid id)
        {
            return await dbContext.EmployeeCafes
                .Include(ec => ec.Employee)
                .Include(ec => ec.Cafe)
                .FirstOrDefaultAsync(ec => ec.Id == id);
        }

        public async Task<IEnumerable<EmployeeCafe>> GetAllAsync()
        {
            return await dbContext.EmployeeCafes
                .Include(ec => ec.Employee)
                .Include(ec => ec.Cafe)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeCafe>> GetByCafeIdAsync(Guid cafeId)
        {
            return await dbContext.EmployeeCafes
                .Where(ec => ec.CafeId == cafeId)
                .Include(ec => ec.Employee)
                .Include(ec => ec.Cafe)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeCafe>> GetByEmployeeIdAsync(string employeeId)
        {
            return await dbContext.EmployeeCafes
                .Where(ec => ec.EmployeeId == employeeId)
                .Include(ec => ec.Employee)
                .Include(ec => ec.Cafe)
                .ToListAsync();
        }

        public async Task<EmployeeCafe> CreateAsync(Guid cafeId, string employeeId, DateTime assignedDate)
        {
            bool employeeExists = await dbContext.Employees.AnyAsync(e => e.Id == employeeId);
            bool cafeExists = await dbContext.Cafes.AnyAsync(c => c.Id == cafeId);

            if (!employeeExists || !cafeExists)
                throw new InvalidOperationException("Employee or Cafe does not exist");

            var employeeCafe = new EmployeeCafe(Guid.NewGuid(), cafeId, employeeId, assignedDate);
            
            await dbContext.EmployeeCafes.AddAsync(employeeCafe);
            await dbContext.SaveChangesAsync();
            
            return employeeCafe;
        }

        public async Task<EmployeeCafe?> UpdateAsync(Guid id, Guid cafeId, string employeeId, bool isActive, DateTime assignedDate)
        {
            var employeeCafe = await dbContext.EmployeeCafes.FindAsync(id);
            if (employeeCafe == null)
                return null;

            bool employeeExists = await dbContext.Employees.AnyAsync(e => e.Id == employeeId);
            bool cafeExists = await dbContext.Cafes.AnyAsync(c => c.Id == cafeId);

            if (!employeeExists || !cafeExists)
                throw new InvalidOperationException("Employee or Cafe does not exist");

            employeeCafe.Update(cafeId, employeeId, isActive, assignedDate);
            dbContext.EmployeeCafes.Update(employeeCafe);
            await dbContext.SaveChangesAsync();
            
            return employeeCafe;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var employeeCafe = await dbContext.EmployeeCafes.FindAsync(id);
            if (employeeCafe == null)
                return false;

            dbContext.EmployeeCafes.Remove(employeeCafe);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await dbContext.EmployeeCafes.AnyAsync(ec => ec.Id == id);
        }

        public async Task<bool> IsEmployeeAssignedToCafeAsync(string employeeId, Guid cafeId)
        {
            return await dbContext.EmployeeCafes
                .AnyAsync(ec => ec.EmployeeId == employeeId && 
                               ec.CafeId == cafeId && 
                               ec.IsActive);
        }
    }
} 