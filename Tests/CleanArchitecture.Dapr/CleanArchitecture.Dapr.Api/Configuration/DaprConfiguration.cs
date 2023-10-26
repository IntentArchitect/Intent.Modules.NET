using CleanArchitecture.Dapr.Application.IntegrationServices;
using CleanArchitecture.Dapr.Infrastructure.HttpClients;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.DaprConfiguration", Version = "1.0")]

namespace CleanArchitecture.Dapr.Api.Configuration
{
    public static class DaprConfiguration
    {
        public static void AddDaprServices(this IServiceCollection services)
        {
            services.AddSingleton<INamedQueryStringsService, NamedQueryStringsServiceHttpClient>(_ => new NamedQueryStringsServiceHttpClient(DaprClient.CreateInvokeHttpClient("clean-architecture-dapr")));
        }
    }
}