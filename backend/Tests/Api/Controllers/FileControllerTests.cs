using Api.Controllers;
using Infrastructure.FileManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json;

namespace Tests.Api.Controllers
{
    public class FileControllerTests
    {
        private readonly Mock<IFileService> fileServiceMock;
        private readonly FileController fileController;

        public FileControllerTests()
        {
            fileServiceMock = new Mock<IFileService>();
            fileController = new FileController(fileServiceMock.Object);
        }

        private Mock<IFormFile> CreateMockImageFile(string fileName = "test.png", string contentType = "image/png", long fileSize = 1000)
        {
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);

            writer.Write(new string('0', (int)fileSize));
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.ContentType).Returns(contentType);

            return fileMock;
        }

        [Fact]
        public async Task UploadLogo_ShouldReturnOk_WhenFileIsValid()
        {
            Mock<IFormFile> fileMock = CreateMockImageFile();
            string expectedFileName = "new-guid.png";

            fileServiceMock.Setup(s => s.UploadLogoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expectedFileName);

            IActionResult result = await fileController.UploadLogo(fileMock.Object);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);


            JsonElement jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(
                System.Text.Json.JsonSerializer.Serialize(okResult.Value));
            
            Assert.True(jsonElement.TryGetProperty("fileName", out var fileNameElement));

            string fileName = fileNameElement.GetString() ?? string.Empty;
            
            Assert.Equal(expectedFileName, fileName);

            fileServiceMock.Verify(s => s.UploadLogoAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task UploadLogo_ShouldReturnBadRequest_WhenFileIsEmpty()
        {
            Mock<IFormFile> fileMock = CreateMockImageFile(fileSize: 0);

            IActionResult result = await fileController.UploadLogo(fileMock.Object);

            Assert.IsType<BadRequestObjectResult>(result);

            fileServiceMock.Verify(s => s.UploadLogoAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task UploadLogo_ShouldReturnBadRequest_WhenFileIsNotImage()
        {
            Mock<IFormFile> fileMock = CreateMockImageFile(contentType: "application/pdf");

            IActionResult result = await fileController.UploadLogo(fileMock.Object);

            Assert.IsType<BadRequestObjectResult>(result);

            fileServiceMock.Verify(s => s.UploadLogoAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public void GetLogo_ShouldReturnFile_WhenFileExists()
        {
            string fileName = "test-logo.png";
            MemoryStream fileStream = new MemoryStream();
            string contentType = "image/png";

            fileServiceMock.Setup(s => s.LogoExists(fileName)).Returns(true);
            fileServiceMock.Setup(s => s.GetLogo(fileName)).Returns((fileStream, contentType));

            IActionResult result = fileController.GetLogo(fileName);

            FileStreamResult fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(contentType, fileResult.ContentType);
            Assert.Equal(fileStream, fileResult.FileStream);

            fileServiceMock.Verify(s => s.LogoExists(fileName), Times.Once);
            fileServiceMock.Verify(s => s.GetLogo(fileName), Times.Once);
        }

        [Fact]
        public void GetLogo_ShouldReturnNotFound_WhenFileDoesNotExist()
        {
            string fileName = "non-existent-logo.png";

            fileServiceMock.Setup(s => s.LogoExists(fileName)).Returns(false);

            IActionResult result = fileController.GetLogo(fileName);

            Assert.IsType<NotFoundObjectResult>(result);

            fileServiceMock.Verify(s => s.LogoExists(fileName), Times.Once);
            fileServiceMock.Verify(s => s.GetLogo(fileName), Times.Never);
        }

        [Fact]
        public void DeleteLogo_ShouldReturnOk_WhenFileIsDeleted()
        {
            string fileName = "test-logo.png";

            fileServiceMock.Setup(s => s.DeleteLogo(fileName)).Returns(true);

            IActionResult result = fileController.DeleteLogo(fileName);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);

            JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult.Value));
            
            Assert.True(jsonElement.TryGetProperty("message", out var messageElement));

            string message = messageElement.GetString() ?? string.Empty;
            
            Assert.Equal("Logo deleted successfully", message);

            fileServiceMock.Verify(s => s.DeleteLogo(fileName), Times.Once);
        }

        [Fact]
        public void DeleteLogo_ShouldReturnNotFound_WhenFileDoesNotExist()
        {
            string fileName = "non-existent-logo.png";

            fileServiceMock.Setup(s => s.DeleteLogo(fileName)).Returns(false);
            fileServiceMock.Setup(s => s.LogoExists(fileName)).Returns(false);

            IActionResult result = fileController.DeleteLogo(fileName);

            Assert.IsType<NotFoundObjectResult>(result);

            fileServiceMock.Verify(s => s.DeleteLogo(fileName), Times.Once);
            fileServiceMock.Verify(s => s.LogoExists(fileName), Times.Once);
        }

        [Fact]
        public void DeleteLogo_ShouldReturnBadRequest_WhenFileNameIsEmpty()
        {
            string fileName = "";

            IActionResult result = fileController.DeleteLogo(fileName);

            Assert.IsType<BadRequestObjectResult>(result);

            fileServiceMock.Verify(s => s.DeleteLogo(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void DeleteLogo_ShouldReturnServerError_WhenDeletionFails()
        {
            string fileName = "test-logo.png";

            fileServiceMock.Setup(s => s.DeleteLogo(fileName)).Returns(false);
            fileServiceMock.Setup(s => s.LogoExists(fileName)).Returns(true);

            IActionResult result = fileController.DeleteLogo(fileName);

            ObjectResult statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            fileServiceMock.Verify(s => s.DeleteLogo(fileName), Times.Once);
            fileServiceMock.Verify(s => s.LogoExists(fileName), Times.Once);
        }
    }
} 