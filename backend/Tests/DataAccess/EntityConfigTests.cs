using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Tests.DataAccess
{
    public class EntityConfigTests : IDisposable
    {
        private readonly AppDbContext appDbContext;

        public EntityConfigTests()
        {
            appDbContext = TestDatabaseHelper.CreateFreshDbContext("happy_cafe_entitytest_test");
        }

        [Fact]
        public void Employee_HasPrimaryKey_Test()
        {
            IEntityType? entityType = appDbContext.Model.FindEntityType(typeof(Employee));

            Assert.NotNull(entityType);
            
            var primaryKey = entityType!.FindPrimaryKey();
            
            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }

        [Fact]
        public void Employee_EmailAddress_IsRequired_Test()
        {
            IEntityType? entityType = appDbContext.Model.FindEntityType(typeof(Employee));

            Assert.NotNull(entityType);
            
            IProperty? emailProperty = entityType!.FindProperty("EmailAddress");

            Assert.NotNull(emailProperty);
            Assert.False(emailProperty!.IsNullable);
        }

        [Fact]
        public void Cafe_Name_IsRequired_Test()
        {
            IEntityType? entityType = appDbContext.Model.FindEntityType(typeof(Cafe));

            Assert.NotNull(entityType);
            
            IProperty? nameProperty = entityType!.FindProperty("Name");
            
            Assert.NotNull(nameProperty);
            Assert.False(nameProperty!.IsNullable);
        }

        [Fact]
        public void EmployeeCafe_HasCorrectRelationships_Test()
        {
            IEntityType? entityType = appDbContext.Model.FindEntityType(typeof(EmployeeCafe));

            Assert.NotNull(entityType);
            
            List<IForeignKey> foreignKeys = entityType!.GetForeignKeys().ToList();
            
            Assert.Equal(2, foreignKeys.Count);
            
            Assert.Contains(foreignKeys, fk => fk.PrincipalEntityType.ClrType == typeof(Employee));
            Assert.Contains(foreignKeys, fk => fk.PrincipalEntityType.ClrType == typeof(Cafe));
        }

        public void Dispose()
        {
            appDbContext.Database.EnsureDeleted();
            appDbContext.Dispose();
        }
    }
} 