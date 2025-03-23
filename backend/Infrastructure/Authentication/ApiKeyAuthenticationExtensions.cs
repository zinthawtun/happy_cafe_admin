using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Authentication
{
    public static class ApiKeyAuthenticationExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
        }
    }
} 