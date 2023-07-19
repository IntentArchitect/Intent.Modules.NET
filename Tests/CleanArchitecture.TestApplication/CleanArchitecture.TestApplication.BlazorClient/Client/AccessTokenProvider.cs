using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace CleanArchitecture.TestApplication.BlazorClient.Client
{
    internal class AccessTokenProvider : IAccessTokenProvider
    {
        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var accessToken = new AccessToken
            {
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
