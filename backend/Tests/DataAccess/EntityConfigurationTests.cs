using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;

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
            var entityType = appDbContext.Model.FindEntityType(typeof(Employee));
            
            Assert.NotNull(entityType);
            
            var primaryKey = entityType!.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
            
            var emailProperty = entityType.FindProperty("EmailAddress");
            Assert.NotNull(emailProperty);
            
            var emailIndex = entityType.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "EmailAddress"));
            if (emailIndex != null)
            {
                Assert.True(emailIndex.IsUnique);
            }
            
            var nameProperty = entityType.FindProperty("Name");
            var phoneProperty = entityType.FindProperty("Phone");
            Assert.NotNull(nameProperty);
            Assert.NotNull(phoneProperty);
            Assert.False(nameProperty!.IsNullable);
            Assert.False(phoneProperty!.IsNullable);
        }
        
        [Fact]
        public void CafeConfiguration_PropertiesAreConfiguredCorrectly_Test()
        {
            var entityType = appDbContext.Model.FindEntityType(typeof(Cafe));
            
            Assert.NotNull(entityType);
            
            var primaryKey = entityType!.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
            
            var nameProperty = entityType.FindProperty("Name");
            var locationProperty = entityType.FindProperty("Location");
            Assert.NotNull(nameProperty);
            Assert.NotNull(locationProperty);
            Assert.False(nameProperty!.IsNullable);
            Assert.False(locationProperty!.IsNullable);
        }
        
        [Fact]
        public void EmployeeCafeConfiguration_RelationshipsAreConfiguredCorrectly_Test()
        {
            var entityType = appDbContext.Model.FindEntityType(typeof(EmployeeCafe));
            
            Assert.NotNull(entityType);
            
            var primaryKey = entityType!.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
            
            var foreignKeys = entityType.GetForeignKeys().ToList();
            
            Assert.Equal(2, foreignKeys.Count);
            Assert.Contains(foreignKeys, fk => fk.PrincipalEntityType.ClrType == typeof(Employee));
            Assert.Contains(foreignKeys, fk => fk.PrincipalEntityType.ClrType == typeof(Cafe));
            
            var employeeFk = foreignKeys.First(fk => fk.PrincipalEntityType.ClrType == typeof(Employee));
            var cafeFk = foreignKeys.First(fk => fk.PrincipalEntityType.ClrType == typeof(Cafe));
            
            Assert.Equal(DeleteBehavior.Cascade, employeeFk.DeleteBehavior);
            Assert.Equal(DeleteBehavior.Cascade, cafeFk.DeleteBehavior);
        }

        public void Dispose()
        {
            appDbContext.Database.EnsureDeleted();
            appDbContext.Dispose();
        }
    }
} 