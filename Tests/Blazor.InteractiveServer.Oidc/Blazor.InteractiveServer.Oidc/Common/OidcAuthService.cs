using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazor.InteractiveServer.Oidc.Components.Account;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.OidcAuthServiceConcreteTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Oidc.Common
{
    internal class OidcAuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityRedirectManager _redirectManager;
        private HttpClient _httpClient;
        private OidcAuthenticationOptions _oidcAuthOptions;

        public OidcAuthService(IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IdentityRedirectManager redirectManager,
            IOptions<OidcAuthenticationOptions> oidcAuthOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient("oidcClient");
            _redirectManager = redirectManager;
            _oidcAuthOptions = oidcAuthOptions.Value;
        }

        public async Task Login(string username, string password, bool rememberMe, string returnUrl)
        {
            if (_oidcAuthOptions is null ||
                string.IsNullOrWhiteSpace(_oidcAuthOptions.ClientId) ||
                string.IsNullOrWhiteSpace(_oidcAuthOptions.ClientSecret) ||
                string.IsNullOrWhiteSpace(_oidcAuthOptions.DefaultScopes))
            {
                throw new InvalidOperationException("OIDC authentication options are not configured correctly.");
            }

            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext found.");
            await httpContext.SignOutAsync();

            var tokenRequest = new Dictionary<string, string>
                                    {
                                        { "grant_type", "password" },
                                        { "client_id", _oidcAuthOptions.ClientId },
                                        { "client_secret", _oidcAuthOptions.ClientSecret },
                                        { "username", username },
                                        { "password", password },
                                        { "scope", _oidcAuthOptions.DefaultScopes }
                                    };
            var tokenResponse = await _httpClient.PostAsJsonAsync("/connect/token", new FormUrlEncodedContent(tokenRequest));

            if (tokenResponse.IsSuccessStatusCode)
            {
                var loginResponse = await tokenResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();
                var claims = new List<Claim>
                                            {
                                                new Claim(ClaimTypes.NameIdentifier, username),
                                                new Claim(ClaimTypes.Email, username),
                                                new Claim("access_token", loginResponse.AccessToken),
                                                new Claim("refresh_token", loginResponse.RefreshToken),
                                                new Claim("token_type", loginResponse.TokenType ?? "Bearer"),
                                                new Claim("expires_at", (loginResponse.ExpiresIn ?? DateTime.UtcNow.AddHours(1)).ToString("o"))

                                            };
                var claimsIdentity = new ClaimsIdentity(claims);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties { IsPersistent = rememberMe });
                _redirectManager.RedirectTo(returnUrl);
            }
            else
            {
                throw new Exception("Error: Invalid login attempt.");
            }
        }
    }
}