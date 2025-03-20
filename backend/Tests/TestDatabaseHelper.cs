using DataAccess;
using DotNetEnv;
using Infrastructure.Configuration;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    public class TestDatabaseConnection : IDatabaseConnection
    {
        private readonly string connectionString;

        public TestDatabaseConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string GetConnectionString() => connectionString;
        public void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder) => 
            optionsBuilder.UseNpgsql(connectionString);
    }

    public static class TestDatabaseHelper
    {
        static TestDatabaseHelper()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            string envFilePath = Configuration.FindEnvFilePath(".env");
            Console.WriteLine($"Loading .env file from: {envFilePath}");

            if (!File.Exists(envFilePath))
            {
                throw new FileNotFoundException($".env file not found at: {envFilePath}");
            }

            Env.Load(envFilePath);
        }

        public static DbContextOptions<AppDbContext> CreateDbContextOptions(string databaseName)
        {
            string? originalDbName = Environment.GetEnvironmentVariable("DB_NAME");
            try
            {
                Environment.SetEnvironmentVariable("DB_NAME", databaseName);
                
                var configuration = new ConfigurationBuilder().Build();
                
                var connectionString = Configuration.GetConnectionString(configuration);
                
                return new DbContextOptionsBuilder<AppDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;
            }
            finally
            {
                Environment.SetEnvironmentVariable("DB_NAME", originalDbName);
            }
        }

        public static AppDbContext CreateFreshDbContext(string databaseName)
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            string username = Environment.GetEnvironmentVariable("DB_USER") ?? "username";
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password";
            
            string testConnectionString = $"Server={host};Port={port};Database={databaseName};User Id={username};Password={password};";
            
            var connection = new TestDatabaseConnection(testConnectionString);
            var context = new AppDbContext(connection);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
} 