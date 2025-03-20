WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.MapOpenApi();

// Add middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();