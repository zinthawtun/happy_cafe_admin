using DataAccess;

namespace Tests.DataAccess
{
    public class DesignTimeDbContextFactoryTests : IDisposable
    {
        private readonly string testProjectRoot;
        private readonly string testConfigPath;
        private readonly string testApiConfigPath;
        private readonly string testBackendApiConfigPath;

        public DesignTimeDbContextFactoryTests()
        {
            testProjectRoot = Path.Combine(Path.GetTempPath(), "TestProject");
            Directory.CreateDirectory(testProjectRoot);
            
            testConfigPath = Path.Combine(testProjectRoot, "appsettings.json");
            testApiConfigPath = Path.Combine(testProjectRoot, "API", "appsettings.json");
            testBackendApiConfigPath = Path.Combine(testProjectRoot, "backend", "API", "appsettings.json");

            File.WriteAllText(testConfigPath, "{\"ConnectionStrings\": {\"DefaultConnection\": \"TestConnection\"}}");
            Directory.CreateDirectory(Path.GetDirectoryName(testApiConfigPath)!);
            File.WriteAllText(testApiConfigPath, "{\"ConnectionStrings\": {\"DefaultConnection\": \"TestApiConnection\"}}");
            Directory.CreateDirectory(Path.GetDirectoryName(testBackendApiConfigPath)!);
            File.WriteAllText(testBackendApiConfigPath, "{\"ConnectionStrings\": {\"DefaultConnection\": \"TestBackendConnection\"}}");
        }

        [Fact]
        public void CreateDbContext_WithValidConfiguration_CreatesDbContext()
        {
            var factory = new DesignTimeDbContextFactory(testProjectRoot);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var dbContext = factory.CreateDbContext(Array.Empty<string>());

            Assert.NotNull(dbContext);
            Assert.IsType<AppDbContext>(dbContext);
        }

        [Fact]
        public void CreateDbContext_WithMissingEnvironment_SetsDevelopmentEnvironment()
        {
            var factory = new DesignTimeDbContextFactory(testProjectRoot);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);

            var dbContext = factory.CreateDbContext(Array.Empty<string>());

            Assert.NotNull(dbContext);
            Assert.Equal("Development", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }

        [Fact]
        public void CreateDbContext_WithMissingConfigFiles_ThrowsException()
        {
            var factory = new DesignTimeDbContextFactory(testProjectRoot);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            File.Delete(testConfigPath);
            File.Delete(testApiConfigPath);
            File.Delete(testBackendApiConfigPath);

            var exception = Assert.Throws<InvalidOperationException>(() => factory.CreateDbContext(Array.Empty<string>()));
            Assert.Equal("No configuration files found.", exception.Message);
        }

        [Fact]
        public void CreateDbContext_WithCustomEnvironment_UsesCorrectEnvironment()
        {
            var factory = new DesignTimeDbContextFactory(testProjectRoot);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

            var dbContext = factory.CreateDbContext(Array.Empty<string>());

            Assert.NotNull(dbContext);
            Assert.Equal("Production", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }

        public void Dispose()
        {
            if (File.Exists(testConfigPath)) File.Delete(testConfigPath);
            if (File.Exists(testApiConfigPath)) File.Delete(testApiConfigPath);
            if (File.Exists(testBackendApiConfigPath)) File.Delete(testBackendApiConfigPath);
            
            if (Directory.Exists(Path.GetDirectoryName(testApiConfigPath)!)) 
                Directory.Delete(Path.GetDirectoryName(testApiConfigPath)!, true);
            if (Directory.Exists(Path.GetDirectoryName(testBackendApiConfigPath)!)) 
                Directory.Delete(Path.GetDirectoryName(testBackendApiConfigPath)!, true);
            if (Directory.Exists(testProjectRoot)) 
                Directory.Delete(testProjectRoot, true);
        }
    }
} 