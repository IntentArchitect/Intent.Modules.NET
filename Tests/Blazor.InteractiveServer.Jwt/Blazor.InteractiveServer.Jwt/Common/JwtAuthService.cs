using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazor.InteractiveServer.Jwt.Components.Account;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.JwtAuthServiceConcreteTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Jwt.Common
{
    internal class JwtAuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityRedirectManager _redirectManager;
        private HttpClient _httpClient;

        public JwtAuthService(IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IdentityRedirectManager redirectManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient("jwtClient");
            _redirectManager = redirectManager;
        }

        public async Task<string> ConfirmEmail(string userId, string code)
        {
            var response = await _httpClient.PostAsJsonAsync("/confirmEmail", new { UserId = userId, Code = code, changedEmail = false });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); ;
            }
            return "Email could not be confirmed";
        }

        public async Task ForgotPassword(string email)
        {
            await _httpClient.PostAsJsonAsync("/forgotPassword", new { Email = email });
            _redirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
        }

        public async Task Login(string username, string password, bool rememberMe, string returnUrl)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext found.");
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var tokenResponse = await _httpClient.PostAsJsonAsync("/login", new { Email = username, Password = password });

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
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent = rememberMe });
                _redirectManager.RedirectTo(returnUrl);
            }
            else
            {
                throw new Exception("Error: Invalid login attempt.");
            }
        }

        public async Task Register(string email, string password, string returnUrl)
        {
            var registerResponse = await _httpClient.PostAsJsonAsync("/register", new { Email = email, Password = password });

            if (!registerResponse.IsSuccessStatusCode)
            {
                throw new Exception("Registration failed");
            }
            _redirectManager.RedirectTo(returnUrl);
        }

        public async Task ResendEmailConfirmation(string email)
        {
            await _httpClient.PostAsJsonAsync("/resendConfirmationEmail", new { Email = email });
        }

        public async Task ResetPassword(string email, string code, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/resetPassword", new { Email = email, ResetCode = code, NewPassword = password });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Reset password failed");
            }
        }
    }
}