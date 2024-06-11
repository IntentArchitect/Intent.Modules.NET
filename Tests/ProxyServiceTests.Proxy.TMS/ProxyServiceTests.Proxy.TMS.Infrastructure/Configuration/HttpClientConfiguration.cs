using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices;
using ProxyServiceTests.Proxy.TMS.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();

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
        }
    }
}