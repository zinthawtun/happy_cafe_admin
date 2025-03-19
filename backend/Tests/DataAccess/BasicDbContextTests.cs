using Business.Entities;
using DataAccess;
using Utilities;

namespace Tests.DataAccess
{
    public class BasicDbContextTests : IDisposable
    {
        private readonly AppDbContext appDbContext;

        public BasicDbContextTests()
        {
            appDbContext = TestDatabaseHelper.CreateFreshDbContext("happy_cafe_basic_test");
        }

        [Fact]
        public void Can_Add_And_Get_Cafe_Test()
        {
            Guid id = Guid.NewGuid();
            Cafe cafe = new Cafe(
                id,
                "Test Cafe",
                "This is a test cafe",
                "logo.png",
                "Test Location"
            );

            appDbContext.Cafes.Add(cafe);
            appDbContext.SaveChanges();

            Cafe? savedCafe = appDbContext.Cafes.Find(id);
            Assert.NotNull(savedCafe);
            Assert.Equal("Test Cafe", savedCafe.Name);
        }

        [Fact]
        public void Can_Add_And_Get_Employee_Test()
        {
            string id = UniqueIdGenerator.GenerateUniqueId();
            Employee employee = new Employee(
                id,
                "John Doe",
                "john.doe@example.com",
                "+1234567890",
                "Male"
            );

            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            Employee? savedEmployee = appDbContext.Employees.Find(id);
            Assert.NotNull(savedEmployee);
            Assert.Equal("John Doe", savedEmployee.Name);
        }

        [Fact]
        public void Can_Add_And_Get_EmployeeCafe_Test()
        {
            Guid cafeId = Guid.NewGuid();
            Cafe? cafe = new Cafe(
                cafeId,
                "Relationship Cafe",
                "For testing relationships",
                "logo.png",
                "Test Location"
            );

            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Employee employee = new Employee(
                employeeId,
                "Jane Smith",
                "jane.smith@example.com",
                "+1234567890",
                "Female"
            );

            appDbContext.Cafes.Add(cafe);
            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();

            Guid relationshipId = Guid.NewGuid();
            EmployeeCafe employeeCafe = new EmployeeCafe(
                relationshipId,
                cafeId,
                employeeId,
                DateTime.UtcNow
            );

            appDbContext.EmployeeCafes.Add(employeeCafe);
            appDbContext.SaveChanges();

            EmployeeCafe? savedRelationship = appDbContext.EmployeeCafes.Find(relationshipId);
            Assert.NotNull(savedRelationship);
            Assert.Equal(cafeId, savedRelationship.CafeId);
            Assert.Equal(employeeId, savedRelationship.EmployeeId);
        }

        public void Dispose()
        {
            appDbContext.Database.EnsureDeleted();
            appDbContext.Dispose();
        }
    }
} 