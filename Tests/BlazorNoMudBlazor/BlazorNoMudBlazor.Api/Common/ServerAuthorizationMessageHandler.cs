using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.ServerAuthorizationMessageHandlerTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Common
{
    public class ServerAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string[] _authorizedUrls;

        public ServerAuthorizationMessageHandler(IHttpContextAccessor httpContextAccessor, string[] authorizedUrls)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizedUrls = authorizedUrls;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            var token = context?.User?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            var requestUrl = request.RequestUri?.AbsoluteUri ?? string.Empty;

            if (!string.IsNullOrEmpty(token) && _authorizedUrls.Any(url => requestUrl.StartsWith(url, StringComparison.OrdinalIgnoreCase)))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}