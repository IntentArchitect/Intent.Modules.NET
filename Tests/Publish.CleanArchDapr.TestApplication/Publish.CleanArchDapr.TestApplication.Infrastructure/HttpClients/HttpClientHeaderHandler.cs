using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientHeaderHandlerTemplate", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Infrastructure.HttpClients
{
    public interface IHeaderConfiguration
    {
        void AddFromHeader(string headerName);
        void AddFromSession(string sessionKey, string headerName);
    }

    public class HttpClientHeaderHandler : DelegatingHandler, IHeaderConfiguration
    {
        private HttpRequestMessage? _request;
        private readonly Action<IHeaderConfiguration> _configureHeaders;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientHeaderHandler(Action<IHeaderConfiguration> configureHeaders,
            IHttpContextAccessor httpContextAccessor)
        {
            _configureHeaders = configureHeaders;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _request = request;
            _configureHeaders(this);

            // Call the base SendAsync method to continue the request
            return await base.SendAsync(request, cancellationToken);
        }

        public void AddFromHeader(string headerName)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }
            if (_httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(headerName, out var value))
            {
                SetHeader(_request!, headerName, value);
            }
        }

        public void AddFromSession(string sessionKey, string headerName)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }
            SetHeader(_request!, headerName, _httpContextAccessor.HttpContext!.Session.GetString(sessionKey));
        }


        private void SetHeader(HttpRequestMessage request, string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            var hasContent = request.Content != null;

            if (!request.Headers.TryGetValues(key, out var _) &&
                !(hasContent && request.Content!.Headers.TryGetValues(key, out var _)))
            {
                if (!request.Headers.TryAddWithoutValidation(key, value) && hasContent)
                {
                    request.Content!.Headers.TryAddWithoutValidation(key, value);
                }
            }
        }

        private void SetHeader(HttpRequestMessage request, string key, StringValues stringValues)
        {
            if (StringValues.IsNullOrEmpty(stringValues))
            {
                return;
            }
            var hasContent = request.Content != null;

            if (!request.Headers.TryGetValues(key, out var _) &&
                !(hasContent && request.Content!.Headers.TryGetValues(key, out var _)))
            {
                if (stringValues.Count == 1)
                {
                    var value = stringValues.ToString();
                    if (!request.Headers.TryAddWithoutValidation(key, value) && hasContent)
                    {
                        request.Content!.Headers.TryAddWithoutValidation(key, value);
                    }
                }
                else
                {
                    var values = stringValues.ToArray();
                    if (!request.Headers.TryAddWithoutValidation(key, (System.Collections.Generic.IEnumerable<string?>)values) && hasContent)
                    {
                        request.Content!.Headers.TryAddWithoutValidation(key, (System.Collections.Generic.IEnumerable<string?>)values);
                    }
                }
            }
        }
    }

    public static class HttpClientExtensions
    {
        public static IHttpClientBuilder AddHeaders(
            this IHttpClientBuilder builder,
            Action<IHeaderConfiguration> configureHeaders)
        {
            builder.AddHttpMessageHandler(services =>
            {
                return new HttpClientHeaderHandler(configureHeaders, services.GetRequiredService<IHttpContextAccessor>());
            });
            return builder;
        }
    }
}