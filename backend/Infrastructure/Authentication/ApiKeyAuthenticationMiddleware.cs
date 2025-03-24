using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Infrastructure.Authentication
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IApiKeyService apiKeyService;

        public ApiKeyAuthenticationMiddleware(RequestDelegate next, IApiKeyService apiKeyService)
        {
            this.next = next;
            this.apiKeyService = apiKeyService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSkipAuthentication(context))
            {
                await next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeaderValues))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"API key is missing\"}");
                return;
            }

            string apiKey = apiKeyHeaderValues.ToString();
            string validApiKey = apiKeyService.GetValidApiKey();

            if (string.IsNullOrEmpty(validApiKey) || apiKey != validApiKey)
            {
                apiKeyService.RefreshApiKey();
                validApiKey = apiKeyService.GetValidApiKey();
                
                if (string.IsNullOrEmpty(validApiKey) || apiKey != validApiKey)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    
                    var errorInfo = new
                    {
                        message = "Invalid API key",
                        status = "error",
                        receivedKeyLength = apiKey?.Length ?? 0,
                        expectedKeyLength = validApiKey?.Length ?? 0,
                        path = context.Request.Path.Value
                    };
                    
                    string errorJson = JsonSerializer.Serialize(errorInfo);
                    await context.Response.WriteAsync(errorJson);
                    return;
                }
            }

            await next(context);
        }

        private bool ShouldSkipAuthentication(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/dev") ||
                context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.Value?.Contains("/swagger/") == true ||
                context.Request.Path.StartsWithSegments("/FileStore/logos"))
            {
                return true;
            }
            
            if (context.Request.Method == HttpMethods.Get && 
                (context.Request.Path.StartsWithSegments("/api/file/logos") ||
                 (context.Request.Path.StartsWithSegments("/api/file") && IsImageFile(context.Request.Path.Value))))
            {
                return true;
            }
            
            return false;
        }
        
        private bool IsImageFile(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
                
            return path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
        }
    }
} 