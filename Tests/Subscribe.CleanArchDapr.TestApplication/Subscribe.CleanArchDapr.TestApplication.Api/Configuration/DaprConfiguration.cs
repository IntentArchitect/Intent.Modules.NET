using Dapr.Client;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.MyProxy;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.DaprConfiguration", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Api.Configuration
{
    public static class DaprConfiguration
    {
        public static void AddDaprServices(this IServiceCollection services)
        {
            services.AddSingleton<IMyProxyClient, MyProxyHttpClient>(_ => new MyProxyHttpClient(DaprClient.CreateInvokeHttpClient("publish-clean-arch-dapr-test-application")));
        }
    }
}