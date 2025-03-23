using Infrastructure.Configuration;
using Infrastructure.FileManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Infrastructure.FileManagement
{
    public class FileServiceTests
    {
        private readonly Mock<IConfiguration> configurationMock;
        private readonly string tempDirectoryPath;
        private readonly FileService fileService;

        public FileServiceTests()
        {
            configurationMock = new Mock<IConfiguration>();
            
            tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectoryPath);
            
            string fileStorePath = Path.Combine(tempDirectoryPath, "FileStore");
            string logosPath = Path.Combine(fileStorePath, "logos");
            
            configurationMock.Setup(c => c["FileStorage:RootPath"]).Returns(tempDirectoryPath);
            configurationMock.Setup(c => c["FileStorage:StorePath"]).Returns(fileStorePath);
            configurationMock.Setup(c => c["FileStorage:LogosPath"]).Returns(logosPath);
            
            fileService = new FileService(configurationMock.Object);
        }
        
        ~FileServiceTests()
        {
            try
            {
                if (Directory.Exists(tempDirectoryPath))
                {
                    Directory.Delete(tempDirectoryPath, true);
                }
            }
            catch
            {
            }
        }

        private Mock<IFormFile> CreateMockImageFile(string fileName = "test.png", string contentType = "image/png", string content = "test file content")
        {
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.ContentType).Returns(contentType);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<Stream, CancellationToken>((stream, token) => {
                    ms.CopyTo(stream);
                })
                .Returns(Task.CompletedTask);
                
            return fileMock;
        }

        private string CreateTestFileInLogosDirectory(string fileName = "test-file.png", string content = "test content")
        {
            string filePath = fileService.GetLogoPath(fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, content);
            return filePath;
        }

        [Fact]
        public void LogoExists_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            string fileName = "non-existent-file.png";
            
            bool exists = fileService.LogoExists(fileName);
            
            Assert.False(exists);
        }

        [Fact]
        public void LogoExists_ShouldReturnFalse_WhenFileNameIsEmpty()
        {
            string fileName = "";
            
            bool exists = fileService.LogoExists(fileName);
            
            Assert.False(exists);
        }

        [Fact]
        public void DeleteLogo_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            string fileName = "non-existent-file.png";
            
            bool deleted = fileService.DeleteLogo(fileName);
            
            Assert.False(deleted);
        }

        [Fact]
        public void DeleteLogo_ShouldReturnFalse_WhenFileNameIsEmpty()
        {
            string fileName = "";
            
            bool deleted = fileService.DeleteLogo(fileName);
            
            Assert.False(deleted);
        }

        [Fact]
        public async Task UploadLogoAsync_ShouldCreateFile_WhenValidFileProvided()
        {
            Mock<IFormFile> fileMock = CreateMockImageFile();
            
            string result = await fileService.UploadLogoAsync(fileMock.Object);
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.EndsWith(".png", result);
            Assert.True(fileService.LogoExists(result));
        }

        [Fact]
        public void GetLogoPath_ShouldReturnValidPath_WhenFileNameProvided()
        {
            string fileName = "test.png";
            
            string path = fileService.GetLogoPath(fileName);
            
            Assert.NotNull(path);
            Assert.NotEmpty(path);
            Assert.EndsWith($"logos{Path.DirectorySeparatorChar}test.png", path);
        }

        [Fact]
        public void DeleteLogo_ShouldReturnTrue_WhenFileExists()
        {
            string fileName = "test-delete.png";
            string filePath = CreateTestFileInLogosDirectory(fileName);
            
            bool deleted = fileService.DeleteLogo(fileName);
            
            Assert.True(deleted);
            Assert.False(File.Exists(filePath));
        }
    }
} 