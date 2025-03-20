using Autofac;
using Microsoft.Extensions.Configuration;

namespace Resource.Registry
{
    public static class ResourceRegistryExtensions
    {
        public static ContainerBuilder RegisterResourceDependencies(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new ResourceRegistry(configuration));
            return builder;
        }
    }
} 