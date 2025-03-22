using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Tests.DataAccess
{
    public class EntityConfigurationTests : IDisposable
    {
        private readonly AppDbContext appDbContext;

        public EntityConfigurationTests()
        {
            appDbContext = TestDatabaseHelper.CreateFreshDbContext("happy_cafe_db_test");
            
            appDbContext.Database.EnsureDeleted();
            appDbContext.Database.EnsureCreated();
        }

        [Fact]
        public void EmployeeConfiguration_PropertiesAreConfiguredCorrectly_Test()
        {
            IEntityType? entityType = appDbContext.Model.FindEntityType(typeof(Employee));
            
            Assert.NotNull(entityType);
            
            IKey? primaryKey = entityType!.FindPrimaryKey();

            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
            
            IProperty? emailProperty = entityType.FindProperty("EmailAddress");

            Assert.NotNull(emailProperty);
            
            IIndex? emailIndex = entityType.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "EmailAddress"));

            if (emailIndex != null)
            {
                Assert.True(emailIndex.IsUnique);
            }
            
            IProperty? nameProperty = entityType.FindProperty("Name");
            IProperty? phoneProperty = entityType.FindProperty("Phone");

            Assert.NotNull(nameProperty);
            Assert.NotNull(phoneProperty);
            Assert.False(nameProperty!.IsNullable);
            Assert.False(phoneProperty!.IsNullable);
        }
        
        [Fact]
        public void CafeConfiguration_PropertiesAreConfiguredCorrectly_Test()
        {
            IEntityType? entityType = appDbContext.Model.FindEntityType(typeof(Cafe));

            Assert.NotNull(entityType);

            IKey? primaryKey = entityType!.FindPrimaryKey();

            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);

            IProperty? nameProperty = entityType.FindProperty("Name");
            IProperty? locationProperty = entityType.FindProperty("Location");

            Assert.NotNull(nameProperty);
            Assert.NotNull(locationProperty);
            Assert.False(nameProperty!.IsNullable);
            Assert.False(locationProperty!.IsNullable);
        }
        
        [Fact]
        public void EmployeeCafeConfiguration_RelationshipsAreConfiguredCorrectly_Test()
        {
            IEntityType? entityType = appDbContext.Model.FindEntityType(typeof(EmployeeCafe));

            Assert.NotNull(entityType);

            IKey? primaryKey = entityType!.FindPrimaryKey();

            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);

            List<IForeignKey> foreignKeys = entityType.GetForeignKeys().ToList();

            Assert.Equal(2, foreignKeys.Count);
            Assert.Contains(foreignKeys, fk => fk.PrincipalEntityType.ClrType == typeof(Employee));
            Assert.Contains(foreignKeys, fk => fk.PrincipalEntityType.ClrType == typeof(Cafe));

            IForeignKey employeeFk = foreignKeys.First(fk => fk.PrincipalEntityType.ClrType == typeof(Employee));
            IForeignKey cafeFk = foreignKeys.First(fk => fk.PrincipalEntityType.ClrType == typeof(Cafe));

            Assert.Equal(DeleteBehavior.Cascade, employeeFk.DeleteBehavior);
            Assert.Equal(DeleteBehavior.Cascade, cafeFk.DeleteBehavior);
        }

        [Fact]
        public void EmployeeConfiguration_EmailAddressFormat_ShouldBeValidated_Test()
        {
            int initialCount = appDbContext.Employees.Count();
            
            Employee validEmployee1 = new Employee(
                "test-id1", 
                "Test Employee", 
                "test@example.com", 
                "89123456", 
                Gender.Male);
            
            Employee validEmployee2 = new Employee(
                "test-id2", 
                "Test Employee 2", 
                "user.name+tag@domain.co.uk", 
                "89123457", 
                Gender.Male);
            
            appDbContext.Employees.Add(validEmployee1);
            appDbContext.Employees.Add(validEmployee2);
            
            appDbContext.SaveChanges();
            
            Assert.Equal(initialCount + 2, appDbContext.Employees.Count());
            
            Employee invalidEmployee1 = new Employee(
                "test-id3", 
                "Invalid Email", 
                "not-an-email",
                "89123458", 
                Gender.Male);
            
            appDbContext.Employees.Add(invalidEmployee1);

            DbUpdateException exception = Assert.Throws<DbUpdateException>(() => appDbContext.SaveChanges());

            string? exceptionMessage = exception.InnerException?.Message;
            Assert.NotNull(exceptionMessage);
            Assert.Contains("CK_Employee_EmailAddress_Format", exceptionMessage);
            
            appDbContext.ChangeTracker.Clear();
            
            Assert.Equal(initialCount + 2, appDbContext.Employees.Count());
            Assert.DoesNotContain(appDbContext.Employees, e => e.Id == "test-id3");
            
            appDbContext.Employees.RemoveRange(
                appDbContext.Employees.Where(e => e.Id == "test-id1" || e.Id == "test-id2")
            );
            appDbContext.SaveChanges();
        }

        public void Dispose()
        {
            appDbContext.Database.EnsureDeleted();
            appDbContext.Dispose();
        }
    }
} 