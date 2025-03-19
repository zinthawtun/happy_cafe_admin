using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private readonly string? baseDirectory;

        public DesignTimeDbContextFactory(string? baseDirectory = null)
        {
            this.baseDirectory = baseDirectory;
        }

        public AppDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrEmpty(environment))
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            }
            
            try
            {
                ConfigurationBuilder configBuilder = new ConfigurationBuilder();
                
                string? projectRoot = baseDirectory ?? Path.GetDirectoryName(AppContext.BaseDirectory);

                while (projectRoot != null && !File.Exists(Path.Combine(projectRoot, "appsettings.json")) && 
                       !Directory.Exists(Path.Combine(projectRoot, "backend")))
                {
                    projectRoot = Directory.GetParent(projectRoot)?.FullName;
                }
                
                bool foundConfigFile = false;
                
                if (projectRoot != null)
                {
                    string? settingsPath = Path.Combine(projectRoot, "appsettings.json");

                    if (File.Exists(settingsPath))
                    {
                        configBuilder.AddJsonFile(settingsPath);
                        Console.WriteLine($"Found and loaded configuration from {settingsPath}");
                        foundConfigFile = true;
                    }
                    
                    string? apiSettingsPath = Path.Combine(projectRoot, "API", "appsettings.json");

                    if (File.Exists(apiSettingsPath))
                    {
                        configBuilder.AddJsonFile(apiSettingsPath);
                        Console.WriteLine($"Found and loaded configuration from {apiSettingsPath}");
                        foundConfigFile = true;
                    }
                    
                    if (Directory.Exists(Path.Combine(projectRoot, "backend")))
                    {
                        string? backendApiSettingsPath = Path.Combine(projectRoot, "backend", "API", "appsettings.json");
                        if (File.Exists(backendApiSettingsPath))
                        {
                            configBuilder.AddJsonFile(backendApiSettingsPath);
                            Console.WriteLine($"Found and loaded configuration from {backendApiSettingsPath}");
                            foundConfigFile = true;
                        }
                    }
                }
                
                if (!foundConfigFile)
                {
                    throw new InvalidOperationException("No configuration files found.");
                }
                
                IConfigurationRoot configuration = configBuilder.Build();

                DatabaseConnection databaseConnection = new Infrastructure.Database.DatabaseConnection(configuration);
                
                databaseConnection.ConfigureDbContext(optionsBuilder);
                
                Console.WriteLine("Successfully configured DbContext using Infrastructure layer");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring DbContext: {ex.Message}");
                throw;
            }

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
