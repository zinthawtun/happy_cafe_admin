using Autofac;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using Resource.Registry;
using Service.Interfaces;
using Service.Mappings;
using Service.Services;
using System.Reflection;

namespace Service.Registry
{
    public class ServiceRegistry : Autofac.Module
    {
        private readonly IConfiguration configuration;

        public ServiceRegistry(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new ResourceRegistry(configuration));

            builder.RegisterType<CafeService>()
                .As<ICafeService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EmployeeService>()
                .As<IEmployeeService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EmployeeCafeService>()
                .As<IEmployeeCafeService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EmployeeCafeIdResolver>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<EmployeeCafeNameResolver>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<CalculateDaysWorkedResolver>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestHandler<>),
                typeof(INotificationHandler<>),
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            builder
                .RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<Profile>())
                .As<Profile>();

            builder.Register(context => new MapperConfiguration(cfg =>
            {
                foreach (Profile profile in context.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                IComponentContext context = c.Resolve<IComponentContext>();
                MapperConfiguration config = context.Resolve<MapperConfiguration>();

                return config.CreateMapper(context.Resolve);
            }).As<IMapper>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            base.Load(builder);
        }
    }

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);

                IEnumerable<ValidationResult> validationResults = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                List<ValidationFailure> failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
} 