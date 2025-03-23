using Infrastructure.FileManagement;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;

        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                if (!file.ContentType.StartsWith("image/"))
                {
                    return BadRequest("Only image files are allowed");
                }

                if (file.Length > 2 * 1024 * 1024)
                {
                    return BadRequest("File size should not exceed 2MB");
                }

                string uniqueFileName = await fileService.UploadLogoAsync(file);

                return Ok(new { fileName = uniqueFileName });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("logos/{fileName}")]
        public IActionResult GetLogo(string fileName)
        {
            try
            {
                if (!fileService.LogoExists(fileName))
                {
                    return NotFound("Logo not found");
                }

                var (fileStream, contentType) = fileService.GetLogo(fileName);
                return File(fileStream, contentType);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("logos/{fileName}")]
        public IActionResult DeleteLogo(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("Filename cannot be empty");
            }

            bool deleted = fileService.DeleteLogo(fileName);
            
            if (!deleted)
            {
                if (!fileService.LogoExists(fileName))
                {
                    return NotFound("Logo not found");
                }
                else
                {
                    return StatusCode(500, "Internal server error while deleting logo");
                }
            }

            return Ok(new { message = "Logo deleted successfully" });
        }
    }
} 