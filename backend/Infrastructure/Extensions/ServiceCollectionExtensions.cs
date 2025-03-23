using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Infrastructure.Database;
using Infrastructure.FileManagement;
using Infrastructure.Authentication;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDatabaseConnection>(new DatabaseConnection(configuration));
            
            services.AddScoped<IFileService, FileService>();

            return services;
        }

        public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
        {
            services.AddSingleton<IApiKeyService, ApiKeyService>();
            
            return services;
        }
    }
} 