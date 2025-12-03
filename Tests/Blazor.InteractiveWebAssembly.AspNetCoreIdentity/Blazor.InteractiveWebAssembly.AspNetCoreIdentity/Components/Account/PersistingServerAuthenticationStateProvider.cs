using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Client.Components.Account.Shared;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.PersistingServerAuthenticationStateProviderTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Components.Account
{
    public class PersistingServerAuthenticationStateProvider : ServerAuthenticationStateProvider, IAccessTokenProvider, IDisposable
    {
        private readonly IdentityOptions options;
        private readonly PersistingComponentStateSubscription subscription;
        private readonly PersistentComponentState _persistentComponentState;
        private readonly IConfiguration _config;
        private Task<AuthenticationState>? authenticationStateTask;

        public PersistingServerAuthenticationStateProvider(PersistentComponentState persistentComponentState,
            IOptions<IdentityOptions> optionsAccessor,
            IConfiguration config)
        {
            _persistentComponentState = persistentComponentState;
            _config = config;
            options = optionsAccessor.Value;
            AuthenticationStateChanged += OnAuthenticationStateChanged;
            subscription = _persistentComponentState.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        }

        public void Dispose()
        {
            subscription.Dispose();
            AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var state = await this.GetAuthenticationStateAsync();
            var token = state.User.FindFirst("access_token");

            if (token == null)
            {
                return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "auth/login", null);
            }
            var accessToken = new AccessToken { Expires = DateTimeOffset.MaxValue, Value = token.Value };
            var result = new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null);
            return result;
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return await RequestAccessToken();
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }

        private async Task OnPersistingAsync()
        {
            if (authenticationStateTask is null)
            {
                throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
            }
            var authenticationState = await authenticationStateTask;
            var principal = authenticationState.User;

            if (principal.Identity?.IsAuthenticated == true)
            {
                var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
                var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;
                var accessToken = principal.FindFirst("access_token")?.Value;
                var refreshToken = principal.FindFirst("refresh_token")?.Value;
                var expiresAtClaim = principal.FindFirst("expires_at")?.Value;
                var refreshUrl = _config.GetValue<string?>("TokenEndpoint:Uri");

                if (!DateTime.TryParse(expiresAtClaim, null, DateTimeStyles.RoundtripKind, out var expiresAt))
                {
                    expiresAt = DateTime.UtcNow.AddHours(1);
                }

                if (userId != null && email != null)
                {
                    var userInfo = new UserInfo { UserId = userId, Email = email, AccessToken = accessToken, RefreshToken = refreshToken, AccessTokenExpiresAt = expiresAt, RefreshUrl = refreshUrl };
                    _persistentComponentState.PersistAsJson(nameof(UserInfo), userInfo);
                }
            }
        }
    }
}