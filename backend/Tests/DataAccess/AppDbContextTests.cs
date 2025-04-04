using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Utilities;
using Xunit;

namespace Tests.DataAccess
{
    public class AppDbContextTests : IDisposable
    {
        private readonly AppDbContext appDbContext;

        public AppDbContextTests()
        {
            appDbContext = TestDatabaseHelper.CreateFreshDbContext("happy_cafe_test");
        }

        [Fact]
        public void CanAddAndRetrieveCafe_Test()
        {
            Cafe cafe = ScenarioHelper.CreateCafe("Test Cafe", "A test cafe for unit testing", "test-logo.png", "Test Location");

            appDbContext.Cafes.Add(cafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? retrievedCafe = appDbContext.Cafes.Find(cafe.Id);
            Assert.NotNull(retrievedCafe);
            Assert.Equal("Test Cafe", retrievedCafe!.Name);
            Assert.Equal("A test cafe for unit testing", retrievedCafe.Description);
            Assert.Equal("test-logo.png", retrievedCafe.Logo);
            Assert.Equal("Test Location", retrievedCafe.Location);
        }

        [Fact]
        public void CanAddAndRetrieveEmployee_Test()
        {
            string uniqueEmail = $"john.doe.unique@example.com";
            Employee employee = ScenarioHelper.CreateEmployee("John Doe", uniqueEmail, "87654321", Gender.Male);

            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Employee? retrievedEmployee = appDbContext.Employees.Find(employee.Id);
            Assert.NotNull(retrievedEmployee);
            Assert.Equal("John Doe", retrievedEmployee!.Name);
            Assert.Equal(uniqueEmail, retrievedEmployee.EmailAddress);
            Assert.Equal("87654321", retrievedEmployee.Phone);
            Assert.Equal(Gender.Male, retrievedEmployee.Gender);
        }

        [Fact]
        public void CanCreateEmployeeCafeRelationship_Test()
        {
            Cafe cafe = ScenarioHelper.CreateCafe("Relationship Test Cafe", "Testing relationships", "logo.png", "Test Location");
            
            string uniqueEmail1 = $"relationship.test.{Guid.NewGuid()}@example.com";
            Employee employee = ScenarioHelper.CreateEmployee("Jane Smith", uniqueEmail1, "98765432", Gender.Female);

            appDbContext.Cafes.Add(cafe);
            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            Guid employeeCafeId = Guid.NewGuid();
            EmployeeCafe employeeCafe = new EmployeeCafe(employeeCafeId, cafe.Id, employee.Id, DateTime.UtcNow);

            appDbContext.EmployeeCafes.Add(employeeCafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? retrievedCafe = appDbContext.Cafes
                .Include(c => c.EmployeeCafes)
                .ThenInclude(ec => ec.Employee)
                .FirstOrDefault(c => c.Id == cafe.Id);

            Assert.NotNull(retrievedCafe);
            Assert.Single(retrievedCafe!.EmployeeCafes);
            Assert.Equal(employee.Id, retrievedCafe.EmployeeCafes.First().EmployeeId);

            Employee? retrievedEmployee = appDbContext.Employees
                .Include(e => e.EmployeeCafes)
                .ThenInclude(ec => ec.Cafe)
                .FirstOrDefault(e => e.Id == employee.Id);

            Assert.NotNull(retrievedEmployee);
            Assert.Single(retrievedEmployee!.EmployeeCafes);
            Assert.Equal(cafe.Id, retrievedEmployee.EmployeeCafes.First().CafeId);
        }

        [Fact]
        public void UpdateCafe_Test()
        {
            Cafe cafe = ScenarioHelper.CreateCafe("Test Cafe", "A test cafe for unit testing", "test-logo.png", "Test Location");

            appDbContext.Cafes.Add(cafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? retrievedCafe = appDbContext.Cafes.Find(cafe.Id);
            Assert.NotNull(retrievedCafe);
            Assert.Equal("Test Cafe", retrievedCafe!.Name);

            retrievedCafe.Update(
                "Updated Test Cafe",
                "An updated test cafe for unit testing",
                "updated-logo.png",
                "Updated Location"
            );
            appDbContext.Cafes.Update(retrievedCafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? updatedCafe = appDbContext.Cafes.Find(cafe.Id);
            Assert.NotNull(updatedCafe);
            Assert.Equal("Updated Test Cafe", updatedCafe!.Name);
        }

        [Fact]
        public void DeleteCafe_Test()
        {
            Cafe cafe = ScenarioHelper.CreateCafe("Test Cafe", "A test cafe for unit testing", "test-logo.png", "Test Location");

            appDbContext.Cafes.Add(cafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? retrievedCafe = appDbContext.Cafes.Find(cafe.Id);
            Assert.NotNull(retrievedCafe);

            appDbContext.Cafes.Remove(retrievedCafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? deletedCafe = appDbContext.Cafes.Find(cafe.Id);
            Assert.Null(deletedCafe);
        }

        [Fact]
        public void UpdateEmployee_Test()
        {
            string uniqueEmail = $"update.test.{Guid.NewGuid()}@happycafe.com";
            Employee employee = ScenarioHelper.CreateEmployee("John Doe", uniqueEmail, "81234567", Gender.Male);

            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Employee? retrievedEmployee = appDbContext.Employees.Find(employee.Id);

            Assert.NotNull(retrievedEmployee);
            Assert.Equal("John Doe", retrievedEmployee!.Name);

            string updatedUniqueEmail = $"jane.smith.{Guid.NewGuid()}@happycafe.com";
            retrievedEmployee.Update(
                "Jane Smith",
                updatedUniqueEmail,
                "99876543",
                Gender.Female
            );

            appDbContext.Employees.Update(retrievedEmployee);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Employee? updatedEmployee = appDbContext.Employees.Find(employee.Id);
            Assert.NotNull(updatedEmployee);
            Assert.Equal("Jane Smith", updatedEmployee!.Name);
            Assert.Equal(updatedUniqueEmail, updatedEmployee.EmailAddress);
            Assert.Equal("99876543", updatedEmployee.Phone);
            Assert.Equal(Gender.Female, updatedEmployee.Gender);               
        }

        [Fact]
        public void DeleteEmployee_Test()
        {
            string uniqueEmail = $"delete.test.{Guid.NewGuid()}@happycafe.com";
            Employee employee = ScenarioHelper.CreateEmployee("John Doe", uniqueEmail, "88889999", Gender.Male);

            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Employee? retrievedEmployee = appDbContext.Employees.Find(employee.Id);

            Assert.NotNull(retrievedEmployee);

            appDbContext.Employees.Remove(retrievedEmployee);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Employee? deletedEmployee = appDbContext.Employees.Find(employee.Id);
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public void DeleteCafeShouldCascadeDeleteEmployeeCafeRelationships_Test()
        {
            Cafe cafe = ScenarioHelper.CreateCafe("Cascade Test Cafe", "Testing cascade delete", "logo.png", "Test Location");
            
            string uniqueEmail1 = $"cascade.jane.{Guid.NewGuid()}@example.com";
            string uniqueEmail2 = $"cascade.john.{Guid.NewGuid()}@example.com";
            
            Employee employee1 = ScenarioHelper.CreateEmployee("Jane Smith", uniqueEmail1, "98765432", Gender.Female);
            Employee employee2 = ScenarioHelper.CreateEmployee("John Doe", uniqueEmail2, "87654321", Gender.Male);

            appDbContext.Cafes.Add(cafe);
            appDbContext.Employees.Add(employee1);
            appDbContext.Employees.Add(employee2);
            appDbContext.SaveChanges();

            appDbContext.EmployeeCafes.Add(new EmployeeCafe(Guid.NewGuid(), cafe.Id, employee1.Id, DateTime.UtcNow));
            appDbContext.EmployeeCafes.Add(new EmployeeCafe(Guid.NewGuid(), cafe.Id, employee2.Id, DateTime.UtcNow));
            appDbContext.SaveChanges();

            int relationshipCount = appDbContext.EmployeeCafes.Count(ec => ec.CafeId == cafe.Id);
            Assert.Equal(2, relationshipCount);

            appDbContext.ChangeTracker.Clear();

            Cafe? cafeToDelete = appDbContext.Cafes.Find(cafe.Id);

            Assert.NotNull(cafeToDelete);
            appDbContext.Cafes.Remove(cafeToDelete!);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Assert.Null(appDbContext.Cafes.Find(cafe.Id));

            Assert.Empty(appDbContext.EmployeeCafes.Where(ec => ec.CafeId == cafe.Id).ToList());
        }

        [Fact]
        public void DeleteEmployeeShouldCascadeDeleteEmployeeCafeRelationships_Test()
        {
            Cafe cafe = ScenarioHelper.CreateCafe("Cascade Test Cafe", "Testing cascade delete", "logo.png", "Test Location");
            
            string uniqueEmail = $"cascade.test.{Guid.NewGuid()}@happycafe.com";
            Employee employee = ScenarioHelper.CreateEmployee("Jane Smith", uniqueEmail, "81122334", Gender.Female);

            appDbContext.Cafes.Add(cafe);

            appDbContext.Employees.Add(employee);

            appDbContext.SaveChanges();

            appDbContext.EmployeeCafes.Add(new EmployeeCafe(Guid.NewGuid(), cafe.Id, employee.Id, DateTime.UtcNow));

            appDbContext.SaveChanges();

            int relationshipCount = appDbContext.EmployeeCafes.Count(ec => ec.EmployeeId == employee.Id);

            Assert.Equal(1, relationshipCount);

            appDbContext.ChangeTracker.Clear();

            Employee? employeeToDelete = appDbContext.Employees.Find(employee.Id);

            Assert.NotNull(employeeToDelete);
            appDbContext.Employees.Remove(employeeToDelete!);

            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Assert.Null(appDbContext.Employees.Find(employee.Id));
            Assert.Empty(appDbContext.EmployeeCafes.Where(ec => ec.EmployeeId == employee.Id).ToList());
        }

        public void Dispose()
        {
            appDbContext.Database.EnsureDeleted();
            appDbContext.Dispose();
        }
    }
} 