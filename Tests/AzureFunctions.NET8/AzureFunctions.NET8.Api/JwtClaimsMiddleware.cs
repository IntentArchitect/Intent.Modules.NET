using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.Jwt.JwtClaimsMiddleware", Version = "1.0")]

namespace AzureFunctions.NET8.Api
{
    public class JwtClaimsMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JwtClaimsMiddleware> _logger;

        public JwtClaimsMiddleware(JwtSecurityTokenHandler tokenHandler,
            IHttpContextAccessor httpContextAccessor,
            ILogger<JwtClaimsMiddleware> logger)
        {
            _tokenHandler = tokenHandler;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (!IsHttpTrigger(context))
            {
                await next(context);
                return;
            }
            var httpContext = context.GetHttpContext();

            if (httpContext == null)
            {
                await next(context);
                return;
            }
            _httpContextAccessor.HttpContext = httpContext;

            if (TryGetBearerToken(httpContext.Request, out var tokenValue))
            {
                if (TryCreatePrincipal(tokenValue, out var principal))
                {
                    AttachPrincipal(httpContext, tokenValue, principal);
                }
                else
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await httpContext.Response.WriteAsync(@"Invalid authorization token.");
                    return;
                }
            }
            await next(context);
        }

        private void AttachPrincipal(HttpContext httpContext, string tokenValue, ClaimsPrincipal principal)
        {
            httpContext.User = principal;
            httpContext.Items["RawToken"] = tokenValue;
        }

        private static bool TryGetBearerToken(HttpRequest request, out string token)
        {
            token = string.Empty;

            if (!request.Headers.TryGetValue(@"Authorization", out StringValues authorizationValues))
            {
                return false;
            }
            var authorizationHeader = authorizationValues.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith(@"Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            token = authorizationHeader[@"Bearer ".Length..].Trim();
            return !string.IsNullOrEmpty(token);
        }

        private static bool IsHttpTrigger(FunctionContext context)
        {
            return context.FunctionDefinition.InputBindings.Values.Any(binding => string.Equals(binding.Type, @"httpTrigger", StringComparison.OrdinalIgnoreCase));
        }

        private bool TryCreatePrincipal(string tokenValue, out ClaimsPrincipal principal)
        {
            principal = default!;

            try
            {
                var token = _tokenHandler.ReadJwtToken(tokenValue);
                var identity = new ClaimsIdentity(token.Claims, @"Bearer", ClaimTypes.Name, ClaimTypes.Role);
                principal = new ClaimsPrincipal(identity);
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, @"Failed to parse JWT token due to invalid format.");
                return false;
            }
        }
    }

    public static class JwtClaimsMiddlewareExtensions
    {
        public static IFunctionsWorkerApplicationBuilder UseJwtClaimsMiddleware(this IFunctionsWorkerApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtClaimsMiddleware>();
        }
    }
}