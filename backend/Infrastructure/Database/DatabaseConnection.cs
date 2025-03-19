using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Database
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly IConfiguration configuration;

        public DatabaseConnection(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetConnectionString()
        {
            return Configuration.Configuration.GetConnectionString(configuration);
        }

        public void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(GetConnectionString());
        }
    }
} 