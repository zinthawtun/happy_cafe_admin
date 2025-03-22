using Business.Entities;
using DataAccess;
using Resource;
using Utilities;

namespace Tests.Resource
{
    public class CafeResourceTests : IDisposable
    {
        private readonly string dbName;
        private readonly AppDbContext context;
        private readonly CafeResource resource;

        public CafeResourceTests()
        {
            dbName = $"CafeResourceDB_Tests";
            context = TestDatabaseHelper.CreateFreshDbContext(dbName);
            resource = new CafeResource(context);
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateCafe_Test()
        {
            Cafe cafe = await resource.CreateAsync(
                "Coffee House", 
                "A cozy coffee shop", 
                "coffee-logo.png", 
                "Downtown");
            
            Assert.NotNull(cafe);
            Assert.NotEqual(Guid.Empty, cafe.Id);
            Assert.Equal("Coffee House", cafe.Name);
            Assert.Equal("A cozy coffee shop", cafe.Description);
            Assert.Equal("coffee-logo.png", cafe.Logo);
            Assert.Equal("Downtown", cafe.Location);
            
            Cafe? savedCafe = await context.Cafes.FindAsync(cafe.Id);
            Assert.NotNull(savedCafe);
            Assert.Equal("Coffee House", savedCafe.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCafes_Test()
        {
            await resource.CreateAsync("Coffee House", "Description 1", "logo1.png", "Location 1");
            await resource.CreateAsync("Tea House", "Description 2", "logo2.png", "Location 2");
            
            List<Cafe> cafes = (await resource.GetAllAsync()).ToList();
            
            Assert.Equal(4, cafes.Count);
            Assert.Contains(cafes, c => c.Name == "Coffee House");
            Assert.Contains(cafes, c => c.Name == "Tea House");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnCafe_Test()
        {
            Cafe created = await resource.CreateAsync("Coffee House", "Description", "logo.png", "Location");
            
            Cafe? cafe = await resource.GetByIdAsync(created.Id);
            
            Assert.NotNull(cafe);
            Assert.Equal(created.Id, cafe.Id);
            Assert.Equal("Coffee House", cafe.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull_Test()
        {
            Cafe? cafe = await resource.GetByIdAsync(Guid.NewGuid());
            
            Assert.Null(cafe);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdateCafe_Test()
        {
            Cafe created = await resource.CreateAsync("Coffee House", "Description", "logo.png", "Location");
            
            Cafe? updated = await resource.UpdateAsync(
                created.Id, 
                "Updated Coffee House", 
                "Updated Description", 
                "updated-logo.png", 
                "New Location");
            
            Assert.NotNull(updated);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal("Updated Coffee House", updated.Name);
            Assert.Equal("Updated Description", updated.Description);
            Assert.Equal("updated-logo.png", updated.Logo);
            Assert.Equal("New Location", updated.Location);
            
            Cafe? savedCafe = await context.Cafes.FindAsync(created.Id);
            Assert.NotNull(savedCafe);
            Assert.Equal("Updated Coffee House", savedCafe.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnNull_Test()
        {
            Cafe? updated = await resource.UpdateAsync(
                Guid.NewGuid(), 
                "Updated Name", 
                "Updated Description", 
                "updated-logo.png", 
                "New Location");
            
            Assert.Null(updated);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_AndNoRelationships_ShouldDeleteCafe_Test()
        {
            Cafe created = await resource.CreateAsync("Coffee House", "Description", "logo.png", "Location");
            
            bool result = await resource.DeleteAsync(created.Id);
            
            Assert.True(result);
            
            Cafe? deletedCafe = await context.Cafes.FindAsync(created.Id);
            Assert.Null(deletedCafe);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse_Test()
        {
            bool result = await resource.DeleteAsync(Guid.NewGuid());
            
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithActiveAssignments_ShouldReturnFalse_Test()
        {
            Cafe cafe = await resource.CreateAsync("Coffee House", "Description", "logo.png", "Location");
            
            Employee employee = new Employee("EMP123", "John Doe", "john@example.com", "89876543", Gender.Male);
            context.Employees.Add(employee);
            
            EmployeeCafe employeeCafe = new EmployeeCafe(Guid.NewGuid(), cafe.Id, employee.Id, DateTime.UtcNow);
            context.EmployeeCafes.Add(employeeCafe);
            await context.SaveChangesAsync();
            
            bool result = await resource.DeleteAsync(cafe.Id);
            
            Assert.False(result);
            
            Cafe? existingCafe = await context.Cafes.FindAsync(cafe.Id);
            Assert.NotNull(existingCafe);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_ShouldReturnCafesAssignedToEmployee_Test()
        {
            Cafe cafe1 = await resource.CreateAsync("Coffee House", "Description 1", "logo1.png", "Location 1");
            Cafe cafe2 = await resource.CreateAsync("Tea House", "Description 2", "logo2.png", "Location 2");
            Cafe cafe3 = await resource.CreateAsync("Juice Bar", "Description 3", "logo3.png", "Location 3");
            
            string eeID = UniqueIdGenerator.GenerateUniqueId();
            Employee employee = new Employee(eeID, "John Doe", "john@example.com", "89876543", Gender.Male);
            context.Employees.Add(employee);
            
            context.EmployeeCafes.Add(new EmployeeCafe(Guid.NewGuid(), cafe1.Id, employee.Id, DateTime.UtcNow));
            
            await context.SaveChangesAsync();
            
            List<Cafe> cafes = (await resource.GetByEmployeeIdAsync(employee.Id)).ToList();
            
            Assert.Single(cafes);
            Assert.Contains(cafes, c => c.Id == cafe1.Id);
            Assert.DoesNotContain(cafes, c => c.Id == cafe3.Id);
        }

        [Fact]
        public async Task ExistsAsync_WithValidId_ShouldReturnTrue_Test()
        {
            Cafe created = await resource.CreateAsync("Coffee House", "Description", "logo.png", "Location");
            
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
        public async Task GetCountAsync_ShouldReturnCorrectCount_Test()
        {
            await resource.CreateAsync("Coffee House", "Description 1", "logo1.png", "Location 1");
            await resource.CreateAsync("Tea House", "Description 2", "logo2.png", "Location 2");
            await resource.CreateAsync("Juice Bar", "Description 3", "logo3.png", "Location 3");
            
            int count = await resource.GetCountAsync();
            
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task GetByLocationLikeAsync_ShouldReturnMatchingCafes_Test()
        {
            await resource.CreateAsync("Downtown Coffee", "Description 1", "logo1.png", "Downtown");
            await resource.CreateAsync("Downtown Tea", "Description 2", "logo2.png", "Downtown");
            await resource.CreateAsync("Uptown Cafe", "Description 3", "logo3.png", "Uptown");
            await resource.CreateAsync("Suburban Coffee", "Description 4", "logo4.png", "Suburban");
            
            List<Cafe> downtownCafes = (await resource.GetByLocationAsync("Downtown")).ToList();
            
            Assert.Equal(2, downtownCafes.Count);
            Assert.All(downtownCafes, cafe => Assert.Equal("Downtown", cafe.Location));
            
            List<Cafe> townCafes = (await resource.GetByLocationAsync("town")).ToList();
            
            Assert.Equal(3, townCafes.Count);
            Assert.Contains(townCafes, c => c.Location == "Downtown");
            Assert.Contains(townCafes, c => c.Location == "Uptown");
            
            List<Cafe> downtownCafesCaseInsensitive = (await resource.GetByLocationAsync("downtown")).ToList();
            
            Assert.Equal(2, downtownCafesCaseInsensitive.Count);
            Assert.All(downtownCafesCaseInsensitive, cafe => Assert.Equal("Downtown", cafe.Location));
            
            List<Cafe> noMatches = (await resource.GetByLocationAsync("Nonexistent")).ToList();
            
            Assert.Empty(noMatches);
        }

        [Fact]
        public async Task ExistsByNameAsync_ShouldReturnTrue_WhenCafeNameExists_Test()
        {
            string cafeName = "Unique Coffee Shop";
            await resource.CreateAsync(cafeName, "Description", "logo.png", "Location");
            
            bool exists = await resource.ExistsByNameAsync(cafeName);
            
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsByNameAsync_ShouldReturnFalse_WhenCafeNameDoesNotExist_Test()
        {
            string nonExistentCafeName = "Non-existent Cafe";
            
            bool exists = await resource.ExistsByNameAsync(nonExistentCafeName);
            
            Assert.False(exists);
        }

        [Fact]
        public async Task ExistsByNameAsync_ShouldBeCaseInsensitive_Test()
        {
            string cafeName = "CamelCase Coffee";
            await resource.CreateAsync(cafeName, "Description", "logo.png", "Location");
            
            bool existsLowerCase = await resource.ExistsByNameAsync("camelcase coffee");
            bool existsUpperCase = await resource.ExistsByNameAsync("CAMELCASE COFFEE");
            bool existsMixedCase = await resource.ExistsByNameAsync("CaMeLcAsE CoFfEe");
            
            Assert.True(existsLowerCase);
            Assert.True(existsUpperCase);
            Assert.True(existsMixedCase);
        }
    }
} 