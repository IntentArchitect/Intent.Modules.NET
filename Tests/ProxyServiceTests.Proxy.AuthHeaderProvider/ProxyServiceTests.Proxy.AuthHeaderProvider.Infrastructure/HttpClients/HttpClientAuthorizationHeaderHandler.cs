using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientAuthorizationHeaderHandler", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.HttpClients
{
    public class HttpClientAuthorizationHeaderHandler : DelegatingHandler
    {
        private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;

        public HttpClientAuthorizationHeaderHandler(IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            _authorizationHeaderProvider = authorizationHeaderProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            SetHeader(request, "Authorization", _authorizationHeaderProvider.GetAuthorizationHeader());

            // Call the base SendAsync method to continue the request
            return await base.SendAsync(request, cancellationToken);
        }

        private void SetHeader(HttpRequestMessage request, string key, string? value)
        {
            var hasContent = request.Content != null;

            if (!request.Headers.TryAddWithoutValidation(key, value) && hasContent)
            {
                request.Content!.Headers.TryAddWithoutValidation(key, value);
            }
        }
    }

    public static class HttpClientAuthorizationHeaderHandlerExtensions
    {
        public static IHttpClientBuilder AddAuthorizationHeader(this IHttpClientBuilder builder)
        {
            builder.AddHttpMessageHandler(services =>
            {
                return new HttpClientAuthorizationHeaderHandler(services.GetRequiredService<IAuthorizationHeaderProvider>());
            });
            return builder;
        }
    }
}