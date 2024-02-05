using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientDaprHandlerTemplate", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Infrastructure.HttpClients
{
    public static class HttpClientDaprExtensions
    {
        public static IHttpClientBuilder ConfigureForDapr(this IHttpClientBuilder builder)
        {
            builder.ConfigureHttpClient(http => http.DefaultRequestHeaders.UserAgent.Add(UserAgent()));
            builder.AddHttpMessageHandler(services =>
            {
                return new InvocationHandler();
            });
            return builder;
        }

        private static ProductInfoHeaderValue UserAgent()
        {
            var assembly = typeof(DaprClient).Assembly;
            string assemblyVersion = assembly
                .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?
                .InformationalVersion;

            return new ProductInfoHeaderValue("dapr-sdk-dotnet", $"v{assemblyVersion}");
        }
    }
}