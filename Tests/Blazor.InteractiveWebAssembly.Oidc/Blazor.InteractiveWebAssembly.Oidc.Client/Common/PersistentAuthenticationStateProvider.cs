using System.Net.Http.Json;
using System.Security.Claims;
using Blazor.InteractiveWebAssembly.Oidc.Client.Components.Account;
using Blazor.InteractiveWebAssembly.Oidc.Client.Components.Account.Shared;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Client.PersistentAuthenticationStateProviderTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Oidc.Client.Common
{
    public class PersistentAuthenticationStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        private static readonly Task<AuthenticationState> _defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        private readonly Task<AuthenticationState> _authenticationStateTask = _defaultUnauthenticatedTask;
        private readonly NavigationManager _nav;
        private string? _accessToken;
        private DateTimeOffset _accessTokenExpiresAt = DateTimeOffset.MinValue;

        public PersistentAuthenticationStateProvider(PersistentComponentState state, NavigationManager nav)
        {
            _nav = nav;
            if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
            {
                return;
            }
            Claim[] claims = [
                                        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                                        new Claim(ClaimTypes.Email, userInfo.Email),
                                        new Claim("access_token", userInfo.AccessToken == null ? "" : userInfo.AccessToken) ];
            _authenticationStateTask = Task.FromResult(
                                        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                                            authenticationType: nameof(PersistentAuthenticationStateProvider)))));

            if (!string.IsNullOrWhiteSpace(userInfo.AccessToken))
            {
                _accessToken = userInfo.AccessToken;

                if (userInfo.AccessTokenExpiresAt.HasValue)
                {
                    _accessTokenExpiresAt = userInfo.AccessTokenExpiresAt.Value;
                }
            }
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return _authenticationStateTask;
        }

        public ValueTask<AccessTokenResult> RequestAccessToken() => RequestAccessToken(new AccessTokenRequestOptions());

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            var missingToken = string.IsNullOrWhiteSpace(_accessToken);
            var expired = _accessTokenExpiresAt > DateTimeOffset.MinValue && _accessTokenExpiresAt <= DateTimeOffset.UtcNow;

            if (missingToken || expired)
            {

                var current = _nav.ToBaseRelativePath(_nav.Uri);
                var returnUrl = "/" + current;
                var loginUrl = "/Account/Login?returnUrl=" + Uri.EscapeDataString(returnUrl);

                return ValueTask.FromResult(
                    new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, loginUrl, null));
            }

            var expires = _accessTokenExpiresAt > DateTimeOffset.MinValue
                            ? _accessTokenExpiresAt
                            : DateTimeOffset.UtcNow.AddMinutes(5);

            var accessToken = new AccessToken
            {
                Value = _accessToken!,
                Expires = expires
            };
            return ValueTask.FromResult(new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null));
        }
    }
}