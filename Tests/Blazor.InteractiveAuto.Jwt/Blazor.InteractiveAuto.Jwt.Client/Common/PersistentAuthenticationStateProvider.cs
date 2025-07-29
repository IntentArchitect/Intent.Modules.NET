using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazor.InteractiveAuto.Jwt.Client.Components.Account.Shared;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Client.PersistentAuthenticationStateProviderTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Jwt.Client.Common
{
    public class PersistentAuthenticationStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        private static readonly Task<AuthenticationState> defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

        public PersistentAuthenticationStateProvider(PersistentComponentState state)
        {
            if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
            {
                return;
            }
            Claim[] claims = [
                                        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                                        new Claim(ClaimTypes.Email, userInfo.Email),
                                        new Claim(ClaimTypes.Email, userInfo.Email),
                                        new Claim("access_token", userInfo.AccessToken == null ? "" : userInfo.AccessToken) ];
            authenticationStateTask = Task.FromResult(
                                        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                                            authenticationType: nameof(PersistentAuthenticationStateProvider)))));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return authenticationStateTask;
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
    }
}