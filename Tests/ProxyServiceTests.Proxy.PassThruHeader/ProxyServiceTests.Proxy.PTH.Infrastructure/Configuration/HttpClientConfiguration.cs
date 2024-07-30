using System;
using System.Net.Http;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices;
using ProxyServiceTests.Proxy.PTH.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services
                .AddHttpClient<IAccountsService, AccountsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "ProxyServiceTests.OriginalServices.Services", "AccountsService");
                });

            services
                .AddHttpClient<IClientsService, ClientsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "ProxyServiceTests.OriginalServices.Services", "ClientsService");
                });

            services
                .AddHttpClient<IDeleteAccountsService, DeleteAccountsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "ProxyServiceTests.OriginalServices.Services", "DeleteAccountsService");
                })
                .AddHeaders(config =>
                {
                    config.AddFromHeader("Authorization");
                });

            services
                .AddHttpClient<IDeleteClientsService, DeleteClientsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "ProxyServiceTests.OriginalServices.Services", "DeleteClientsService");
                })
                .AddHeaders(config =>
                {
                    config.AddFromHeader("Authorization");
                });
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