using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public interface IDbContext
    {
        void EnsureCreated();
    }
} 