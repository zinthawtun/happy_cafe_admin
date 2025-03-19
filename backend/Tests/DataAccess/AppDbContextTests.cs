using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Utilities;

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
            Guid cafeId = Guid.NewGuid();
            Cafe cafe = new Cafe(
                cafeId,
                "Test Cafe",
                "A test cafe for unit testing",
                "test-logo.png",
                "Test Location"
            );

            appDbContext.Cafes.Add(cafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? retrievedCafe = appDbContext.Cafes.Find(cafeId);
            Assert.NotNull(retrievedCafe);
            Assert.Equal("Test Cafe", retrievedCafe!.Name);
            Assert.Equal("A test cafe for unit testing", retrievedCafe.Description);
            Assert.Equal("test-logo.png", retrievedCafe.Logo);
            Assert.Equal("Test Location", retrievedCafe.Location);
        }

        [Fact]
        public void CanAddAndRetrieveEmployee_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Employee employee = new Employee(
                employeeId,
                "John Doe",
                "john.doe@example.com",
                "+1234567890",
                "Male"
            );

            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Employee? retrievedEmployee = appDbContext.Employees.Find(employeeId);
            Assert.NotNull(retrievedEmployee);
            Assert.Equal("John Doe", retrievedEmployee!.Name);
            Assert.Equal("john.doe@example.com", retrievedEmployee.EmailAddress);
            Assert.Equal("+1234567890", retrievedEmployee.Phone);
            Assert.Equal("Male", retrievedEmployee.Gender);
        }

        [Fact]
        public void CanCreateEmployeeCafeRelationship_Test()
        {
            Guid cafeId = Guid.NewGuid();
            Cafe cafe = new Cafe(
                cafeId,
                "Relationship Test Cafe",
                "Testing relationships",
                "logo.png",
                "Test Location"
            );

            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Employee employee = new Employee(
                employeeId,
                "Jane Smith",
                "jane.smith@example.com",
                "+0987654321",
                "Female"
            );

            appDbContext.Cafes.Add(cafe);
            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            Guid employeeCafeId = Guid.NewGuid();
            EmployeeCafe employeeCafe = new EmployeeCafe(employeeCafeId, cafeId, employeeId, DateTime.UtcNow);

            appDbContext.EmployeeCafes.Add(employeeCafe);
            appDbContext.SaveChanges();

            appDbContext.ChangeTracker.Clear();

            Cafe? retrievedCafe = appDbContext.Cafes
                .Include(c => c.EmployeeCafes)
                .ThenInclude(ec => ec.Employee)
                .FirstOrDefault(c => c.Id == cafeId);

            Assert.NotNull(retrievedCafe);
            Assert.Single(retrievedCafe!.EmployeeCafes);
            Assert.Equal(employeeId, retrievedCafe.EmployeeCafes.First().EmployeeId);

            Employee? retrievedEmployee = appDbContext.Employees
                .Include(e => e.EmployeeCafes)
                .ThenInclude(ec => ec.Cafe)
                .FirstOrDefault(e => e.Id == employeeId);

            Assert.NotNull(retrievedEmployee);
            Assert.Single(retrievedEmployee!.EmployeeCafes);
            Assert.Equal(cafeId, retrievedEmployee.EmployeeCafes.First().CafeId);
        }

        public void Dispose()
        {
            appDbContext.Database.EnsureDeleted();
            appDbContext.Dispose();
        }
    }
} 