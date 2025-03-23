using Microsoft.Extensions.Configuration;

namespace Infrastructure.Authentication
{
    public interface IApiKeyService
    {
        string GetValidApiKey();
        void RefreshApiKey();
    }

    public class ApiKeyService : IApiKeyService, IDisposable
    {
        private readonly IConfiguration configuration;
        private string _cachedApiKey;
        private readonly SemaphoreSlim apiKeySemaphore = new SemaphoreSlim(1, 1);
        private FileSystemWatcher? fileWatcher;
        private readonly string setupDevEnvPath;
        private readonly string localEnvPath;

        public ApiKeyService(IConfiguration configuration)
        {
            this.configuration = configuration;
            
            try
            {
                setupDevEnvPath = Configuration.Configuration.FindEnvFilePath();
                localEnvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing API key paths: {ex.Message}");
                setupDevEnvPath = string.Empty;
                localEnvPath = string.Empty;
            }
            
            _cachedApiKey = LoadApiKey();
            SetupFileWatcher();
        }

        public string GetValidApiKey()
        {
            if (string.IsNullOrEmpty(_cachedApiKey))
            {
                RefreshApiKey();
            }
            return _cachedApiKey;
        }

        public void RefreshApiKey()
        {
            apiKeySemaphore.Wait();
            try
            {
                LoadEnvironmentVariables();
                
                string? apiKey = Environment.GetEnvironmentVariable("Authentication__ApiKey");
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    apiKey = configuration["Authentication:ApiKey"];
                }
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _cachedApiKey = apiKey;
                    Console.WriteLine($"API key refreshed. Length: {apiKey.Length}");
                }
                else
                {
                    Console.WriteLine("Failed to refresh API key - no valid key found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing API key: {ex.Message}");
            }
            finally
            {
                apiKeySemaphore.Release();
            }
        }

        private void LoadEnvironmentVariables()
        {
            try
            {
                if (!string.IsNullOrEmpty(setupDevEnvPath) && File.Exists(setupDevEnvPath))
                {
                    Console.WriteLine($"Loading env from: {setupDevEnvPath}");
                    DotNetEnv.Env.Load(setupDevEnvPath);
                }

                if (!string.IsNullOrEmpty(localEnvPath) && File.Exists(localEnvPath))
                {
                    Console.WriteLine($"Loading env from: {localEnvPath}");
                    DotNetEnv.Env.Load(localEnvPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading environment variables: {ex.Message}");
            }
        }

        private string LoadApiKey()
        {
            LoadEnvironmentVariables();
            
            string? envValue = Environment.GetEnvironmentVariable("Authentication__ApiKey");

            if (!string.IsNullOrEmpty(envValue))
            {
                return envValue;
            }
            
            string? configValue = configuration["Authentication:ApiKey"];

            return configValue ?? string.Empty;
        }

        private void SetupFileWatcher()
        {
            try
            {
                if (!string.IsNullOrEmpty(setupDevEnvPath))
                {
                    string? directoryPath = Path.GetDirectoryName(setupDevEnvPath);
                    if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                    {
                        fileWatcher = new FileSystemWatcher(directoryPath);
                        fileWatcher.Filter = "*.env";
                        fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
                        fileWatcher.Changed += OnEnvFileChanged;
                        fileWatcher.Created += OnEnvFileChanged;
                        fileWatcher.EnableRaisingEvents = true;
                        Console.WriteLine($"File watcher set up for: {directoryPath}");
                    }
                }

                if (!string.IsNullOrEmpty(localEnvPath) && File.Exists(localEnvPath))
                {
                    string? localDir = Path.GetDirectoryName(localEnvPath);
                    if (!string.IsNullOrEmpty(localDir) && Directory.Exists(localDir))
                    {
                        var localWatcher = new FileSystemWatcher(localDir);
                        localWatcher.Filter = ".env";
                        localWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
                        localWatcher.Changed += OnEnvFileChanged;
                        localWatcher.Created += OnEnvFileChanged;
                        localWatcher.EnableRaisingEvents = true;
                        Console.WriteLine($"File watcher set up for: {localDir}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up file watcher: {ex.Message}");
            }
        }

        private void OnEnvFileChanged(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(500);
            
            Console.WriteLine($"Detected change in .env file: {e.FullPath}");
            RefreshApiKey();
            Console.WriteLine($"API key refreshed due to .env file change");
        }

        public void Dispose()
        {
            fileWatcher?.Dispose();
        }
    }
} 