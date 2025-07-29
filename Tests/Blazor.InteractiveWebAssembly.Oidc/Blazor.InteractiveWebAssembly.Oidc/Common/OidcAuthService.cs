using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazor.InteractiveWebAssembly.Oidc.Components.Account;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.OidcAuthServiceConcreteTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Oidc.Common
{
    internal class OidcAuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityRedirectManager _redirectManager;
        private HttpClient _httpClient;

        public OidcAuthService(IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IdentityRedirectManager redirectManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient("oidcClient");
            _redirectManager = redirectManager;
        }

        public async Task Login(string username, string password, bool rememberMe, string returnUrl)
        {
            var tokenResponse = await _httpClient.PostAsJsonAsync("/login", new { Email = username, Password = password });

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokens = await tokenResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();
                var claims = new List<Claim>
                                            {
                                                new Claim(ClaimTypes.NameIdentifier, username),
                                                new Claim(ClaimTypes.Email, username),
                                                new Claim("access_token", tokens.AccessToken),
                                                new Claim("refresh_token", tokens.RefreshToken),
                                                new Claim("token_type", tokens.TokenType),
                                                new Claim("expires_at", tokens.ExpiresIn.ToString())
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

        public class AccessTokenResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string TokenType { get; set; }
            public DateTime ExpiresIn { get; set; }
        }
    }
}