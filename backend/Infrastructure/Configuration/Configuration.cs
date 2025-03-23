using Infrastructure.FileManagement;
using Microsoft.Extensions.Configuration;
using Utilities;

namespace Infrastructure.Configuration
{
    public static class Configuration
    {
        public static string FindEnvFilePath(string fileName = ".env")
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string setupFilePath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" 
                ? Constants.SETUP_DEV_FILE_PATH 
                : Constants.SETUP_PROD_FILE_PATH;

            while (!Directory.Exists(Path.Combine(currentDir, "setup_dev")) && Directory.GetParent(currentDir) != null)
            {
                currentDir = Directory.GetParent(currentDir)!.FullName;
            }

            if (!Directory.Exists(Path.Combine(currentDir, "setup_dev")))
            {
                throw new DirectoryNotFoundException("Could not find setup_dev directory in any parent directory");
            }

            return Path.Combine(currentDir, "setup_dev", fileName);
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
            {
                string envPath = FindEnvFilePath();
                if (!File.Exists(envPath))
                {
                    throw new FileNotFoundException($".env file not found at: {envPath}");
                }

                DotNetEnv.Env.Load(envPath);

                string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
                string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
                string database = Environment.GetEnvironmentVariable("DB_NAME") ?? "cafe_employee_db";
                string username = Environment.GetEnvironmentVariable("DB_USER") ?? "username";
                string password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password";

                return $"Server={host};Port={port};Database={database};User Id={username};Password={password};";
            }

            return configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public static FileConfiguration GetFileStorageConfiguration(IConfiguration configuration)
        {
            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
            {
                string envPath = FindEnvFilePath();
                if (!File.Exists(envPath))
                {
                    throw new FileNotFoundException($".env file not found at: {envPath}");
                }

                DotNetEnv.Env.Load(envPath);

                string rootPath = Environment.GetEnvironmentVariable("FILE_STORAGE_ROOT_PATH") ?? Directory.GetCurrentDirectory();
                string storePath = Environment.GetEnvironmentVariable("FILE_STORAGE_PATH") ?? "FileStore";
                string logosPath = Environment.GetEnvironmentVariable("FILE_STORAGE_LOGOS_PATH") ?? "FileStore/logos";
                
                long maxFileSize = 2 * 1024 * 1024;
                if (long.TryParse(Environment.GetEnvironmentVariable("FILE_STORAGE_MAX_SIZE"), out long parsedSize))
                {
                    maxFileSize = parsedSize;
                }

                string fileStorePath = storePath.StartsWith(rootPath) ? storePath : Path.Combine(rootPath, storePath);
                string fullLogosPath = logosPath.StartsWith(rootPath) ? logosPath : Path.Combine(rootPath, logosPath);

                return new FileConfiguration
                {
                    RootPath = rootPath,
                    FileStorePath = fileStorePath,
                    LogosPath = fullLogosPath,
                    MaxFileSize = maxFileSize
                };
            }

            return new FileConfiguration
            {
                RootPath = configuration["FileStorage:RootPath"] ?? Directory.GetCurrentDirectory(),
                FileStorePath = configuration["FileStorage:StorePath"] ?? Path.Combine(configuration["FileStorage:RootPath"] ?? Directory.GetCurrentDirectory(), "FileStore"),
                LogosPath = configuration["FileStorage:LogosPath"] ?? Path.Combine(configuration["FileStorage:RootPath"] ?? Directory.GetCurrentDirectory(), "FileStore/logos"),
                MaxFileSize = configuration.GetValue<long>("FileStorage:MaxFileSize", 2 * 1024 * 1024)
            };
        }

        public static void AddApiKeyConfiguration(IConfigurationBuilder configBuilder)
        {
            string? apiKey = Environment.GetEnvironmentVariable("Authentication__ApiKey");
            if (!string.IsNullOrEmpty(apiKey))
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Authentication:ApiKey", apiKey }
                });
                return;
            }

            try
            {
                string envPath = FindEnvFilePath();
                if (File.Exists(envPath))
                {
                    DotNetEnv.Env.Load(envPath);
                }

                string localEnvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
                if (File.Exists(localEnvPath))
                {
                    DotNetEnv.Env.Load(localEnvPath);
                }

                apiKey = Environment.GetEnvironmentVariable("Authentication__ApiKey");
                if (!string.IsNullOrEmpty(apiKey))
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Authentication:ApiKey", apiKey }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading API key from environment: {ex.Message}");
            }
        }
    }
}
