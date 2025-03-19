using System;
using System.Linq;
using Business.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Tests;

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
            var entityType = appDbContext.Model.FindEntityType(typeof(Employee));
            Assert.NotNull(entityType);
            
            var primaryKey = entityType!.FindPrimaryKey();
            
            Assert.NotNull(primaryKey);
            Assert.Single(primaryKey.Properties);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }

        [Fact]
        public void Employee_EmailAddress_IsRequired_Test()
        {
            var entityType = appDbContext.Model.FindEntityType(typeof(Employee));
            Assert.NotNull(entityType);
            
            var emailProperty = entityType!.FindProperty("EmailAddress");

            Assert.NotNull(emailProperty);
            Assert.False(emailProperty!.IsNullable);
        }

        [Fact]
        public void Cafe_Name_IsRequired_Test()
        {
            var entityType = appDbContext.Model.FindEntityType(typeof(Cafe));
            Assert.NotNull(entityType);
            
            var nameProperty = entityType!.FindProperty("Name");
            
            Assert.NotNull(nameProperty);
            Assert.False(nameProperty!.IsNullable);
        }

        [Fact]
        public void EmployeeCafe_HasCorrectRelationships_Test()
        {
            var entityType = appDbContext.Model.FindEntityType(typeof(EmployeeCafe));
            Assert.NotNull(entityType);
            
            var foreignKeys = entityType!.GetForeignKeys().ToList();
            
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