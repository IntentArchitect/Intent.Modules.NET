using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace CleanArchitecture.Comprehensive.BlazorClient.Client
{
    internal class AccessTokenProvider : IAccessTokenProvider
    {
        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var accessToken = new AccessToken
            {
                Expires = DateTimeOffset.MaxValue,
                Value = "<your access token here>"
            };

            var result = new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null);

            return ValueTask.FromResult(result);
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return await RequestAccessToken();
        }
    }
}
