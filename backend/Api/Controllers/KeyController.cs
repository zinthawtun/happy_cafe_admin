using Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/dev/key")]
    public class KeyController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly IApiKeyService apiKeyService;

        public KeyController(IWebHostEnvironment environment, IApiKeyService apiKeyService)
        {
            this.environment = environment;
            this.apiKeyService = apiKeyService;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateDevApiKey()
        {
            if (!environment.IsDevelopment())
            {
                return StatusCode(403, new { Message = "This operation is only allowed in Development environment" });
            }

            try
            {
                string apiKey = GenerateRandomApiKey();
                
                string projectRoot = GetProjectRoot();
                string setupDevEnvPath = Path.Combine(projectRoot, "setup_dev", ".env");
                
                if (System.IO.File.Exists(setupDevEnvPath))
                {
                    await UpdateEnvFile(setupDevEnvPath, "Authentication__ApiKey", apiKey);
                }
                else
                {
                    return BadRequest(new { Message = $"setup_dev/.env file not found. Looked in: {setupDevEnvPath}" });
                }
                
                string frontendEnvPath = Path.Combine(projectRoot, "frontend", ".env");
                
                if (System.IO.File.Exists(frontendEnvPath))
                {
                    await UpdateEnvFile(frontendEnvPath, "VITE_API_KEY", apiKey);
                }
                else
                {
                    return BadRequest(new { Message = $"frontend/.env file not found. Looked in: {frontendEnvPath}" });
                }
                
                apiKeyService.RefreshApiKey();
                
                return Ok(new { 
                    ApiKey = apiKey,
                    Message = "API key generated and saved to environment files successfully",
                    UpdatedFiles = new[] { setupDevEnvPath, frontendEnvPath }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error generating API key: {ex.Message}", StackTrace = ex.StackTrace });
            }
        }
        

        private string GenerateRandomApiKey()
        {
            const int keyLength = 32;
            byte[] randomBytes = new byte[keyLength];
            
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            
            string apiKey = Convert.ToBase64String(randomBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 32); 
            
            return apiKey;
        }

        private string GetProjectRoot()
        {
            string currentDir = Directory.GetCurrentDirectory();
            
            while (!string.IsNullOrEmpty(currentDir) && !Directory.Exists(Path.Combine(currentDir, "setup_dev")))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(currentDir);
                if (dirInfo.Parent == null)
                {
                    throw new DirectoryNotFoundException($"Could not find the project root directory containing 'setup_dev'. Started from: {Directory.GetCurrentDirectory()}");
                }
                currentDir = dirInfo.Parent.FullName;
            }
            
            if (string.IsNullOrEmpty(currentDir))
            {
                throw new DirectoryNotFoundException($"Could not find the project root directory containing 'setup_dev'. Started from: {Directory.GetCurrentDirectory()}");
            }
            
            return currentDir;
        }
        
        private async Task UpdateEnvFile(string filePath, string key, string value)
        {
            string content = await System.IO.File.ReadAllTextAsync(filePath);
            string pattern = $"^{Regex.Escape(key)}=.*$";
            string replacement = $"{key}={value}";
            
            bool keyExists = Regex.IsMatch(content, pattern, RegexOptions.Multiline);
            
            if (keyExists)
            {
                content = Regex.Replace(content, pattern, replacement, RegexOptions.Multiline);
            }
            else
            {
                content += Environment.NewLine + replacement;
            }
            
            await System.IO.File.WriteAllTextAsync(filePath, content);
        }

        [HttpPost("refresh-api-key")]
        public IActionResult RefreshApiKey()
        {
            try
            {
                apiKeyService.RefreshApiKey();
                string currentKey = apiKeyService.GetValidApiKey();
                bool hasKey = !string.IsNullOrEmpty(currentKey);
                
                return Ok(new { 
                    Success = hasKey,
                    Message = hasKey ? "API key refreshed successfully" : "Failed to refresh API key - no valid key found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error refreshing API key: {ex.Message}" });
            }
        }
        
        [HttpGet("debug")]
        public IActionResult DebugApiKey()
        {
            try
            {
                string currentKey = apiKeyService.GetValidApiKey();
                bool hasKey = !string.IsNullOrEmpty(currentKey);
                
                return Ok(new { 
                    HasKey = hasKey,
                    KeyLength = currentKey?.Length ?? 0,
                    EnvVarExists = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Authentication__ApiKey")),
                    EnvVarLength = Environment.GetEnvironmentVariable("Authentication__ApiKey")?.Length ?? 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error getting API key debug info: {ex.Message}" });
            }
        }
    }
} 