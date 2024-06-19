using System;
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
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:AccountsService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:AccountsService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IClientsService, ClientsServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:ClientsService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:ClientsService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IDeleteAccountsService, DeleteAccountsServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:DeleteAccountsService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:DeleteAccountsService:Timeout") ?? TimeSpan.FromSeconds(100);
                })
                .AddHeaders(config =>
                {
                    config.AddFromHeader("Authorization");
                });

            services
                .AddHttpClient<IDeleteClientsService, DeleteClientsServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:DeleteClientsService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:DeleteClientsService:Timeout") ?? TimeSpan.FromSeconds(100);
                })
                .AddHeaders(config =>
                {
                    config.AddFromHeader("Authorization");
                });
        }
    }
}