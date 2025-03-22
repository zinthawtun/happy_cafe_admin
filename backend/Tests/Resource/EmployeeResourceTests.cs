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
    public class EmployeeResourceTests : IDisposable
    {
        private readonly string dbName;
        private readonly AppDbContext context;
        private readonly EmployeeResource resource;

        public EmployeeResourceTests()
        {
            dbName = $"EmployeeResourceDB_Tests";
            context = TestDatabaseHelper.CreateFreshDbContext(dbName);
            resource = new EmployeeResource(context);
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateEmployee_Test()
        {
            Employee employee = await resource.CreateAsync(
                "John Doe",
                "john.doe2@example.com",
                "89123456",
                Gender.Male);
            
            Assert.NotNull(employee);
            Assert.NotEmpty(employee.Id);
            Assert.Equal("John Doe", employee.Name);
            Assert.Equal("john.doe2@example.com", employee.EmailAddress);
            Assert.Equal("89123456", employee.Phone);
            Assert.Equal(Gender.Male, employee.Gender);
            
            Employee? savedEmployee = await context.Employees.FindAsync(employee.Id);
            Assert.NotNull(savedEmployee);
            Assert.Equal("John Doe", savedEmployee.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployees_Test()
        {
            List<Employee> employees = (await resource.GetAllAsync()).ToList();

            Assert.Equal(2, employees.Count);

            await resource.CreateAsync("John Doe", "john.doe7@example.com", "89123457", Gender.Male);
            await resource.CreateAsync("Jane Smith", "jane.smith7@example.com", "90123457", Gender.Female);
            
            employees = (await resource.GetAllAsync()).ToList();
            
            Assert.Equal(4, employees.Count);
            Assert.Contains(employees, e => e.Name == "John Doe");
            Assert.Contains(employees, e => e.Name == "Jane Smith");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEmployee_Test()
        {
            Employee created = await resource.CreateAsync("John Doe", "john.doe8@example.com", "89123458", Gender.Male);
            
            Employee? employee = await resource.GetByIdAsync(created.Id);
            
            Assert.NotNull(employee);
            Assert.Equal(created.Id, employee.Id);
            Assert.Equal("John Doe", employee.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull_Test()
        {
            Employee? employee = await resource.GetByIdAsync("nonexistent-id");
            
            Assert.Null(employee);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdateEmployee_Test()
        {
            Employee created = await resource.CreateAsync("John Doe", "john.doe0@example.com", "89123410", Gender.Male);
            
            Employee? updated = await resource.UpdateAsync(
                created.Id,
                "John Updated",
                "john.updated0@example.com",
                "90123410",
                Gender.Male);
            
            Assert.NotNull(updated);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal("John Updated", updated.Name);
            Assert.Equal("john.updated0@example.com", updated.EmailAddress);
            Assert.Equal("90123410", updated.Phone);
            
            Employee? savedEmployee = await context.Employees.FindAsync(created.Id);
            Assert.NotNull(savedEmployee);
            Assert.Equal("John Updated", savedEmployee.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnNull_Test()
        {
            Employee? updated = await resource.UpdateAsync(
                "nonexistent-id",
                "John Updated",
                "john.updated@example.com",
                "89876543",
                Gender.Male);
            
            Assert.Null(updated);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_AndNoRelationships_ShouldDeleteEmployee_Test()
        {
            Employee created = await resource.CreateAsync("John Doe", "john.doe4@example.com", "89123454", Gender.Male);
            
            bool result = await resource.DeleteAsync(created.Id);
            
            Assert.True(result);
            
            Employee? deletedEmployee = await context.Employees.FindAsync(created.Id);
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse_Test()
        {
            bool result = await resource.DeleteAsync("nonexistent-id");
            
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithActiveAssignments_ShouldReturnFalse_Test()
        {
            Employee employee = await resource.CreateAsync("John Doe", "john.doe3@example.com", "89123453", Gender.Male);
            
            Cafe cafe = new Cafe(Guid.NewGuid(), "Test Cafe", "Description", "logo.png", "Location");
            context.Cafes.Add(cafe);
            
            EmployeeCafe employeeCafe = new EmployeeCafe(Guid.NewGuid(), cafe.Id, employee.Id, DateTime.UtcNow);
            context.EmployeeCafes.Add(employeeCafe);
            await context.SaveChangesAsync();
            
            bool result = await resource.DeleteAsync(employee.Id);
            
            Assert.False(result);
            
            Employee? existingEmployee = await context.Employees.FindAsync(employee.Id);
            Assert.NotNull(existingEmployee);
        }

        [Fact]
        public async Task GetByCafeIdAsync_ShouldReturnEmployeesAssignedToCafe_Test()
        {
            Employee employee1 = await resource.CreateAsync("John Doe", "john@example.com", "89123450", Gender.Male);
            Employee employee2 = await resource.CreateAsync("Jane Smith", "jane@example.com", "90123451", Gender.Female);
            Employee employee3 = await resource.CreateAsync("Bob Johnson", "bob@example.com", "89123452", Gender.Male);
            
            Cafe cafe = new Cafe(Guid.NewGuid(), "Test Cafe", "Description", "logo.png", "Location");
            context.Cafes.Add(cafe);
            
            context.EmployeeCafes.Add(new EmployeeCafe(Guid.NewGuid(), cafe.Id, employee1.Id, DateTime.UtcNow));
            context.EmployeeCafes.Add(new EmployeeCafe(Guid.NewGuid(), cafe.Id, employee2.Id, DateTime.UtcNow));
            
            await context.SaveChangesAsync();
            
            List<Employee> employees = (await resource.GetByCafeIdAsync(cafe.Id)).ToList();
            
            Assert.Equal(2, employees.Count);
            Assert.Contains(employees, e => e.Id == employee1.Id);
            Assert.Contains(employees, e => e.Id == employee2.Id);
            Assert.DoesNotContain(employees, e => e.Id == employee3.Id);
        }

        [Fact]
        public async Task ExistsAsync_WithValidId_ShouldReturnTrue_Test()
        {
            Employee created = await resource.CreateAsync("John Doe", "john.doe5@example.com", "89123455", Gender.Male);
            
            bool exists = await resource.ExistsAsync(created.Id);
            
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse_Test()
        {
            bool exists = await resource.ExistsAsync("nonexistent-id");
            
            Assert.False(exists);
        }

        [Fact]
        public async Task GetCountAsync_ShouldReturnCorrectCount_Test()
        {
            int count = await resource.GetCountAsync();

            Assert.Equal(2, count);

            await resource.CreateAsync("John Doe", "john.doe6@example.com", "89123456", Gender.Male);
            await resource.CreateAsync("Jane Smith", "jane.smith6@example.com", "90123456", Gender.Female);
            await resource.CreateAsync("Bob Johnson", "bob.johnson6@example.com", "89123446", Gender.Male);
            
            count = await resource.GetCountAsync();
            
            Assert.Equal(5, count);
        }
    }
} 