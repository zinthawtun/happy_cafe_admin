using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.FileManagement
{
    public interface IFileService
    {
        Task<string> UploadLogoAsync(IFormFile file);

        string GetLogoPath(string fileName);

        (Stream FileStream, string ContentType) GetLogo(string fileName);

        bool DeleteLogo(string fileName);

        bool LogoExists(string fileName);
    }
} 