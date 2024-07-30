using System;
using System.Net.Http;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<IAccountsService, AccountsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "ProxyServiceTests.OriginalServices.Services", "AccountsService");
                })
                .AddAuthorizationHeader();

            services
                .AddHttpClient<IClientsService, ClientsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "ProxyServiceTests.OriginalServices.Services", "ClientsService");
                })
                .AddAuthorizationHeader();
        }

        private static void ApplyAppSettings(
            HttpClient client,
            IConfiguration configuration,
            string groupName,
            string serviceName)
        {
            client.BaseAddress = configuration.GetValue<Uri>($"HttpClients:{serviceName}:Uri") ?? configuration.GetValue<Uri>($"HttpClients:{groupName}:Uri");
            client.Timeout = configuration.GetValue<TimeSpan?>($"HttpClients:{serviceName}:Timeout") ?? configuration.GetValue<TimeSpan?>($"HttpClients:{groupName}:Timeout") ?? TimeSpan.FromSeconds(100);
        }
    }
}