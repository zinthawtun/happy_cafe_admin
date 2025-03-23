using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Resource;
using Resource.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Utilities;

namespace Tests.Resource
{
    public class EmployeeCafeResourceTests : IDisposable
    {
        private readonly string dbName;
        private readonly AppDbContext context;
        private readonly EmployeeCafeResource resource;
        private readonly string employeeId;

        public EmployeeCafeResourceTests()
        {
            dbName = $"EmployeeCafeResourceDB_Tests";
            context = TestDatabaseHelper.CreateFreshDbContext(dbName);
            resource = new EmployeeCafeResource(context);

            employeeId = UniqueIdGenerator.GenerateUniqueId();
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateEmployeeCafe_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe = new Cafe(Guid.NewGuid(), "Coffee House", "Description", "logo.png", "Location");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe);
            await context.SaveChangesAsync();
            
            EmployeeCafe employeeCafe = await resource.CreateAsync(
                cafe.Id, 
                employee.Id, 
                DateTime.UtcNow);
            
            Assert.NotNull(employeeCafe);
            Assert.NotEqual(Guid.Empty, employeeCafe.Id);
            Assert.Equal(cafe.Id, employeeCafe.CafeId);
            Assert.Equal(employee.Id, employeeCafe.EmployeeId);
            Assert.True(employeeCafe.IsActive);
            
            EmployeeCafe? savedAssignment = await context.EmployeeCafes
                .Include(ec => ec.Employee)
                .Include(ec => ec.Cafe)
                .FirstOrDefaultAsync(ec => ec.Id == employeeCafe.Id);
                
            Assert.NotNull(savedAssignment);
            Assert.Equal(employee.Id, savedAssignment.EmployeeId);
            Assert.Equal(cafe.Id, savedAssignment.CafeId);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidEmployeeId_ShouldThrowException_Test()
        {
            Cafe cafe = new Cafe(Guid.NewGuid(), "Coffee House", "Description", "logo.png", "Location");
            context.Cafes.Add(cafe);
            await context.SaveChangesAsync();
            
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                resource.CreateAsync(cafe.Id, "invalid-employee-id", DateTime.UtcNow));
        }

        [Fact]
        public async Task CreateAsync_WithInvalidCafeId_ShouldThrowException_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                resource.CreateAsync(Guid.NewGuid(), employee.Id, DateTime.UtcNow));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployeeCafeAssignments_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john1@example.com", "89123457", Gender.Male);
            Employee employee1 = new Employee(UniqueIdGenerator.GenerateUniqueId(), "Jane Smith", "jane.smith1@example.com", "90123457", Gender.Female);
            Cafe cafe1 = new Cafe(Guid.NewGuid(), "Coffee House", "Description 1", "logo1.png", "Location 1");
            Cafe cafe2 = new Cafe(Guid.NewGuid(), "Tea House", "Description 2", "logo2.png", "Location 2");
            
            context.Employees.Add(employee);
            context.Employees.Add(employee1);
            context.Cafes.Add(cafe1);
            context.Cafes.Add(cafe2);

            context.SaveChanges();
            
            await resource.CreateAsync(cafe1.Id, employee.Id, DateTime.UtcNow);
            await resource.CreateAsync(cafe2.Id, employee1.Id, DateTime.UtcNow);
            
            List<EmployeeCafe> assignments = (await resource.GetAllAsync()).ToList();
            
            Assert.Equal(4, assignments.Count);
            Assert.Contains(assignments, a => a.CafeId == cafe1.Id);
            Assert.Contains(assignments, a => a.CafeId == cafe2.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmployeeOrCafeNotExitError_Test()
        {
            Cafe cafe1 = new Cafe(Guid.NewGuid(), "Coffee House", "Description 1", "logo1.png", "Location 1");
            Cafe cafe2 = new Cafe(Guid.NewGuid(), "Tea House", "Description 2", "logo2.png", "Location 2");

            context.Cafes.Add(cafe1);
            context.Cafes.Add(cafe2);
            await context.SaveChangesAsync();

            InvalidOperationException? exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                resource.CreateAsync(cafe1.Id, "invalid-employee-id", DateTime.UtcNow));

            Assert.Equal("Employee or Cafe does not exist", exception.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEmployeeCafe_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe = new Cafe(Guid.NewGuid(), "Coffee House", "Description", "logo.png", "Location");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe);

            await context.SaveChangesAsync();

            EmployeeCafe created = await resource.CreateAsync(cafe.Id, employee.Id, DateTime.UtcNow);
            
            EmployeeCafe? employeeCafe = await resource.GetByIdAsync(created.Id);
            
            Assert.NotNull(employeeCafe);
            Assert.Equal(created.Id, employeeCafe.Id);
            Assert.Equal(cafe.Id, employeeCafe.CafeId);
            Assert.Equal(employee.Id, employeeCafe.EmployeeId);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull_Test()
        {
            EmployeeCafe? employeeCafe = await resource.GetByIdAsync(Guid.NewGuid());
            
            Assert.Null(employeeCafe);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdateEmployeeCafe_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe1 = new Cafe(Guid.NewGuid(), "Coffee House", "Description 1", "logo1.png", "Location 1");
            Cafe cafe2 = new Cafe(Guid.NewGuid(), "Tea House", "Description 2", "logo2.png", "Location 2");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe1);
            context.Cafes.Add(cafe2);

            await context.SaveChangesAsync();

            EmployeeCafe created = await resource.CreateAsync(cafe1.Id, employee.Id, DateTime.UtcNow);
            
            EmployeeCafe? updated = await resource.UpdateAsync(
                created.Id,
                cafe2.Id,
                employee.Id,
                false,
                DateTime.UtcNow.AddDays(1));
            
            Assert.NotNull(updated);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal(cafe2.Id, updated.CafeId);
            Assert.False(updated.IsActive);
            Assert.Equal(DateTime.UtcNow.AddDays(1).Date, updated.AssignedDate.Date);
            
            EmployeeCafe? savedAssignment = await context.EmployeeCafes.FindAsync(created.Id);
            Assert.NotNull(savedAssignment);
            Assert.Equal(cafe2.Id, savedAssignment.CafeId);
            Assert.False(savedAssignment.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnNull_Test()
        {
            EmployeeCafe? updated = await resource.UpdateAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                UniqueIdGenerator.GenerateUniqueId(),
                true,
                DateTime.UtcNow);
            
            Assert.Null(updated);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteEmployeeCafe_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe = new Cafe(Guid.NewGuid(), "Coffee House", "Description", "logo.png", "Location");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe);

            await context.SaveChangesAsync();

            EmployeeCafe created = await resource.CreateAsync(cafe.Id, employee.Id, DateTime.UtcNow);
            
            bool result = await resource.DeleteAsync(created.Id);
            
            Assert.True(result);
            
            EmployeeCafe? deletedAssignment = await context.EmployeeCafes.FindAsync(created.Id);
            Assert.Null(deletedAssignment);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse_Test()
        {
            bool result = await resource.DeleteAsync(Guid.NewGuid());
            
            Assert.False(result);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_ShouldReturnAllAssignmentsForEmployee_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe1 = new Cafe(Guid.NewGuid(), "Coffee House", "Description 1", "logo1.png", "Location 1");
            Cafe cafe2 = new Cafe(Guid.NewGuid(), "Tea House", "Description 2", "logo2.png", "Location 2");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe1);
            context.Cafes.Add(cafe2);

            await context.SaveChangesAsync();
            await resource.CreateAsync(cafe1.Id, employee.Id, DateTime.UtcNow);
            await resource.CreateAsync(cafe2.Id, employee.Id, DateTime.UtcNow);
            
            List<EmployeeCafe> assignments = (await resource.GetByEmployeeIdAsync(employee.Id)).ToList();
            
            Assert.Equal(2, assignments.Count);
            Assert.Contains(assignments, a => a.CafeId == cafe1.Id);
            Assert.Contains(assignments, a => a.CafeId == cafe2.Id);
        }

        [Fact]
        public async Task ExistsAsync_WithValidId_ShouldReturnTrue_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe = new Cafe(Guid.NewGuid(), "Coffee House", "Description", "logo.png", "Location");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe);

            await context.SaveChangesAsync();

            EmployeeCafe created = await resource.CreateAsync(cafe.Id, employee.Id, DateTime.UtcNow);
            
            bool exists = await resource.ExistsAsync(created.Id);
            
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse_Test()
        {
            bool exists = await resource.ExistsAsync(Guid.NewGuid());
            
            Assert.False(exists);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_WithValidAssignment_ShouldReturnTrue_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe = new Cafe(Guid.NewGuid(), "Coffee House", "Description", "logo.png", "Location");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe);

            await context.SaveChangesAsync();
            await resource.CreateAsync(cafe.Id, employee.Id, DateTime.UtcNow);
            
            bool isAssigned = await resource.IsEmployeeAssignedToCafeAsync(employee.Id, cafe.Id);
            
            Assert.True(isAssigned);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_WithInactiveAssignment_ShouldReturnFalse_Test()
        {
            Employee employee = new Employee(employeeId, "John Doe", "john@example.com", "89123456", Gender.Male);
            Cafe cafe = new Cafe(Guid.NewGuid(), "Coffee House", "Description", "logo.png", "Location");
            
            context.Employees.Add(employee);
            context.Cafes.Add(cafe);
            await context.SaveChangesAsync();

            EmployeeCafe created = await resource.CreateAsync(cafe.Id, employee.Id, DateTime.UtcNow);
            await resource.UpdateAsync(created.Id, cafe.Id, employee.Id, false, DateTime.UtcNow);
            
            bool isAssigned = await resource.IsEmployeeAssignedToCafeAsync(employee.Id, cafe.Id);
            
            Assert.False(isAssigned);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_WithNoAssignment_ShouldReturnFalse_Test()
        {
            bool isAssigned = await resource.IsEmployeeAssignedToCafeAsync(UniqueIdGenerator.GenerateUniqueId(), Guid.NewGuid());
            
            Assert.False(isAssigned);
        }
    }
} 