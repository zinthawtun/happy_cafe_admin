using Autofac;
using DataAccess;
using DataAccess.Registry;
using Microsoft.Extensions.Configuration;
using Resource.Interfaces;

namespace Resource.Registry
{
    public class ResourceRegistry : Module
    {
        private readonly IConfiguration configuration;

        public ResourceRegistry(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DataAccessRegistry(configuration));

            builder.RegisterType<EmployeeResource>()
                .As<IEmployeeResource>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CafeResource>()
                .As<ICafeResource>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EmployeeCafeResource>()
                .As<IEmployeeCafeResource>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
} 