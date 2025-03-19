using Microsoft.Extensions.Configuration;
using Utilities;

namespace Infrastructure.Configuration
{
    public static class Configuration
    {
        public static string FindEnvFilePath(string fileName = ".env")
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string setupFilePath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? Constants.SETUP_DEV_FILE_PATH : Constants.SETUP_PROD_FILE_PATH;

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
    }
}
