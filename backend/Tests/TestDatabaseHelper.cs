using DataAccess;
using DotNetEnv;
using Infrastructure.Configuration;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            string dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "username";
            string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password";

            return new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql($"Server={host};Port={port};Database={databaseName};User Id={dbUser};Password={dbPassword}")
                .Options;
        }

        public static AppDbContext CreateFreshDbContext(string databaseName)
        {
            string connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"};" +
                                 $"Port={Environment.GetEnvironmentVariable("DB_PORT") ?? "5432"};" +
                                 $"Database={databaseName};" +
                                 $"User Id={Environment.GetEnvironmentVariable("DB_USER") ?? "username"};" +
                                 $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password"}";
            
            var connection = new TestDatabaseConnection(connectionString);
            var context = new AppDbContext(connection);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
} 