using Autofac;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Registry
{
    public static class DataAccessRegistryExtensions
    {
        public static ContainerBuilder RegisterDataAccessDependencies(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new DataAccessRegistry(configuration));
            return builder;
        }
    }
} 