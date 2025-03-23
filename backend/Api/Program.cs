using Api.Registry;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Service.Registry;
using Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Happy Cafe Admin API", Version = "v1" });
});

builder.Services.AddDirectoryBrowser();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureApiServices();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterService(builder.Configuration);
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Happy Cafe Admin API v1"));

app.UseCors("AllowFrontend");

app.UseStaticFiles(); 

string rootPath = Directory.GetCurrentDirectory();
string fileStorePath = Path.Combine(rootPath, "FileStore");
string logosPath = Path.Combine(fileStorePath, "logos");

if (!Directory.Exists(fileStorePath))
{
    Directory.CreateDirectory(fileStorePath);
    Console.WriteLine($"Created directory: {fileStorePath}");
}

if (!Directory.Exists(logosPath))
{
    Directory.CreateDirectory(logosPath);
    Console.WriteLine($"Created directory: {logosPath}");
}

app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Happy Cafe Admin API is running!");
app.MapGet("/api/test", () => new { Message = "API is working correctly" });

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request path: {context.Request.Path}");
    await next.Invoke();
});

app.Run();