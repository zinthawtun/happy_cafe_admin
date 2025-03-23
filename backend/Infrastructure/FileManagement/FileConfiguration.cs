namespace Infrastructure.FileManagement
{
    public class FileConfiguration
    {
        public string RootPath { get; set; } = string.Empty;

        public string FileStorePath { get; set; } = string.Empty;

        public string LogosPath { get; set; } = string.Empty;

        public long MaxFileSize { get; set; } = 2 * 1024 * 1024;
    }
} 