using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;

namespace DataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>((sp, options) => 
            {
                var databaseConnection = sp.GetRequiredService<IDatabaseConnection>();
                databaseConnection.ConfigureDbContext(options);
            });
            
            services.AddScoped<IDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            
            return services;
        }
    }
} 