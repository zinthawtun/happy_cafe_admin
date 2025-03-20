using Autofac;
using Infrastructure.Database;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Registry
{
    public class DataAccessRegistry : Module
    {
        private readonly IConfiguration configuration;

        public DataAccessRegistry(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppDbContext>()
                .AsSelf()
                .As<IDbContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DatabaseConnection>()
                .As<IDatabaseConnection>()
                .WithParameter(new TypedParameter(typeof(IConfiguration), configuration))
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
} 