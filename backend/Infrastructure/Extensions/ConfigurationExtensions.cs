using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddApiKeyFromEnvironment(this IConfigurationBuilder configBuilder)
        {
            Configuration.Configuration.AddApiKeyConfiguration(configBuilder);
            return configBuilder;
        }
    }
} 