using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Utilities;

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
            Cafe cafe = ScenarioHelper.CreateCafe("Happy Integration Cafe", "A cafe for integration testing", "integration-logo.png", "Integration Test Location");
            testCafeID = cafe.Id;
            
            Employee employee = ScenarioHelper.CreateEmployee("Integration Tester", "integration.tester@example.com", "89123456", Gender.Male);
            testEmployeeID = employee.Id;
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

        [Fact]
        public async Task CanDeleteCafe_Test()
        {
            Cafe? cafe = await appDbContext.Cafes.FindAsync(testCafeID);
            Assert.NotNull(cafe);
            appDbContext.Cafes.Remove(cafe!);
            await appDbContext.SaveChangesAsync();
            Cafe? deletedCafe = await appDbContext.Cafes.FindAsync(testCafeID);
            Assert.Null(deletedCafe);
        }

        [Fact]
        public async Task CanDeleteEmployee_Test()
        {
            Employee? employee = await appDbContext.Employees.FindAsync(testEmployeeID);
            Assert.NotNull(employee);
            appDbContext.Employees.Remove(employee!);
            await appDbContext.SaveChangesAsync();
            Employee? deletedEmployee = await appDbContext.Employees.FindAsync(testEmployeeID);
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public async Task CanDeleteEmployeeCafeRelationshipsByEmployeeId_Test()
        {
            List<EmployeeCafe> deletedEmployeeCafes = await appDbContext.EmployeeCafes
                .Where(ec => ec.EmployeeId == testEmployeeID)
                .ToListAsync();

            Assert.Empty(deletedEmployeeCafes);
        }

        [Fact]
        public async Task CanDeleteEmployeeCafeRelationshipsByCafeId_Test()
        {
            List<EmployeeCafe> deletedEmployeeCafes = await appDbContext.EmployeeCafes
                .Where(ec => ec.CafeId == testCafeID)
                .ToListAsync();
            Assert.Empty(deletedEmployeeCafes);
        }

        [Fact]
        public async Task SaveEmployeeAndFindById_ShouldWork_Test()
        {
            Employee employee = ScenarioHelper.CreateEmployee("Integration Tester", "integration.tester1@example.com", "89123456", Gender.Male);
            
            appDbContext.Employees.Add(employee);
            await appDbContext.SaveChangesAsync();
            
            Employee? foundEmployee = await appDbContext.Employees.FindAsync(employee.Id);
            
            Assert.NotNull(foundEmployee);
            Assert.Equal(employee.Id, foundEmployee.Id);
            Assert.Equal("Integration Tester", foundEmployee.Name);
            Assert.Equal("integration.tester1@example.com", foundEmployee.EmailAddress);
            Assert.Equal("89123456", foundEmployee.Phone);
            Assert.Equal(Gender.Male, foundEmployee.Gender);
        }

        public async Task DisposeAsync()
        {
            await appDbContext.Database.EnsureDeletedAsync();
            await appDbContext.DisposeAsync();
        }
    }
} 