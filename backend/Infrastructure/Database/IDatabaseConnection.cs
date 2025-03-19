using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public interface IDatabaseConnection
    {
        string GetConnectionString();
        void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder);
    }
} 