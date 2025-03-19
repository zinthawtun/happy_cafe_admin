using DataAccess.Extensions;
using Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Add data access services
builder.Services.AddDataAccess();

WebApplication app = builder.Build();

app.MapOpenApi();

// Add middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();