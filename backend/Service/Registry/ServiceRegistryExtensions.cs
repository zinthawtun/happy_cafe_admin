using Autofac;
using Microsoft.Extensions.Configuration;

namespace Service.Registry
{
    public static class ServiceRegistryExtensions
    {
        public static ContainerBuilder RegisterService(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new ServiceRegistry(configuration));
            return builder;
        }
    }
} 