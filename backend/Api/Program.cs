using Api.Registry;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Service.Registry;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
ApiRegistry.AddOpenApi(builder.Services);
builder.Services.ConfigureApiServices();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterService(builder.Configuration);
});

WebApplication app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();