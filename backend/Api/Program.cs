using Api.Registry;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Service.Registry;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger generation service directly
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Happy Cafe Admin API", Version = "v1" });
});

// Configure other API services
builder.Services.ConfigureApiServices();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterService(builder.Configuration);
});

WebApplication app = builder.Build();

// Configure Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Happy Cafe Admin API v1"));

app.UseAuthorization();
app.MapControllers();

// Add test endpoints
app.MapGet("/", () => "Happy Cafe Admin API is running!");
app.MapGet("/api/test", () => new { Message = "API is working correctly" });

// Debug middleware to log request paths
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request path: {context.Request.Path}");
    await next.Invoke();
});

app.Run();