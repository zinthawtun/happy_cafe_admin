using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Infrastructure.Configuration;

namespace Infrastructure.FileManagement
{
    public class FileService : IFileService
    {
        private readonly FileConfiguration fileConfig;

        public FileService(IConfiguration configuration)
        {
            fileConfig = Configuration.Configuration.GetFileStorageConfiguration(configuration);

            EnsureDirectoryExists(fileConfig.FileStorePath);
            EnsureDirectoryExists(fileConfig.LogosPath);
        }

        public async Task<string> UploadLogoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException("Only image files are allowed");
            }

            if (file.Length > fileConfig.MaxFileSize)
            {
                throw new ArgumentException($"File size should not exceed {fileConfig.MaxFileSize / (1024 * 1024)}MB");
            }

            string fileName = Path.GetFileName(file.FileName);
            string fileExtension = Path.GetExtension(fileName);
            string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(fileConfig.LogosPath, uniqueFileName);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        public string GetLogoPath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Filename cannot be empty");
            }

            return Path.Combine(fileConfig.LogosPath, fileName);
        }

        public (Stream FileStream, string ContentType) GetLogo(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Filename cannot be empty");
            }

            string filePath = GetLogoPath(fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Logo not found: {fileName}");
            }

            string contentType = GetContentType(fileName);
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            return (fileStream, contentType);
        }

        public bool DeleteLogo(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            try
            {
                string filePath = GetLogoPath(fileName);

                if (!File.Exists(filePath))
                {
                    return false;
                }

                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool LogoExists(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            string filePath = GetLogoPath(fileName);
            return File.Exists(filePath);
        }

        private string GetContentType(string fileName)
        {
            string contentType = "application/octet-stream";
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".bmp":
                    contentType = "image/bmp";
                    break;
                case ".webp":
                    contentType = "image/webp";
                    break;
            }

            return contentType;
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
} 