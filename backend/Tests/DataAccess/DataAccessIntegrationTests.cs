using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Utilities;
using Tests;

namespace Tests.DataAccess
{
    public class DataAccessIntegrationTests : IAsyncLifetime
    {
        private AppDbContext appDbContext = null!;
        private Guid testCafeID;
        private string testEmployeeID = null!;

        public async Task InitializeAsync()
        {
            appDbContext = TestDatabaseHelper.CreateFreshDbContext("happy_cafe_integration_test");
            await SeedTestDataAsync();
        }

        private async Task SeedTestDataAsync()
        {
            testCafeID = Guid.NewGuid();
            Cafe cafe = new Cafe(
                testCafeID,
                "Happy Integration Cafe",
                "A cafe for integration testing",
                "integration-logo.png",
                "Integration Test Location"
            );
            
            testEmployeeID = UniqueIdGenerator.GenerateUniqueId();
            Employee employee = new Employee(
                testEmployeeID,
                "Integration Tester",
                "integration.tester@example.com",
                "+1234567890",
                "Male"
            );
            
            await appDbContext.Cafes.AddAsync(cafe);
            await appDbContext.Employees.AddAsync(employee);
            await appDbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CanQueryCafesByLocation_Test()
        {
            List<Cafe> cafes = await appDbContext.Cafes
                .Where(c => c.Location.Contains("Integration"))
                .ToListAsync();
            
            Assert.Single(cafes);
            Assert.Equal("Happy Integration Cafe", cafes.First().Name);
        }
        
        [Fact]
        public async Task CanCreateAndQueryEmployeeCafeRelationship_Test()
        {
            Guid relationshipId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow;
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                relationshipId,
                testCafeID,
                testEmployeeID,
                assignedDate
            );
            
            await appDbContext.EmployeeCafes.AddAsync(employeeCafe);
            await appDbContext.SaveChangesAsync();
            
            Employee? employee = await appDbContext.Employees
                .Include(e => e.EmployeeCafes)
                .ThenInclude(ec => ec.Cafe)
                .FirstOrDefaultAsync(e => e.Id == testEmployeeID);
            
            Assert.NotNull(employee);
            Assert.Single(employee!.EmployeeCafes);
            Assert.Equal(testCafeID, employee.EmployeeCafes.First().CafeId);
            Assert.Equal("Happy Integration Cafe", employee.EmployeeCafes.First().Cafe!.Name);
        }
        
        [Fact]
        public async Task CanUpdateCafeProperties_Test()
        {
            Cafe? cafe = await appDbContext.Cafes.FindAsync(testCafeID);
            Assert.NotNull(cafe);
            
            cafe!.Update(
                "Updated Integration Cafe",
                "Updated description for integration testing",
                "updated-logo.png",
                "Updated Integration Location"
            );
            
            await appDbContext.SaveChangesAsync();
            
            appDbContext.ChangeTracker.Clear();
            
            Cafe? updatedCafe = await appDbContext.Cafes.FindAsync(testCafeID);
            Assert.NotNull(updatedCafe);
            Assert.Equal("Updated Integration Cafe", updatedCafe!.Name);
            Assert.Equal("Updated description for integration testing", updatedCafe.Description);
            Assert.Equal("updated-logo.png", updatedCafe.Logo);
            Assert.Equal("Updated Integration Location", updatedCafe.Location);
        }

        public async Task DisposeAsync()
        {
            await appDbContext.Database.EnsureDeletedAsync();
            await appDbContext.DisposeAsync();
        }
    }
} 