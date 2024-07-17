using System;
using System.Net.Http;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices;
using Standard.AspNetCore.TestApplication.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure.Configuration
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
                .AddHttpClient<IIntegrationServiceProxy, IntegrationServiceProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "Standard.AspNetCore.TestApplication.Services", "IntegrationServiceProxy");
                })
                .AddClientAccessTokenHandler(configuration.GetValue<string>("HttpClients:IntegrationServiceProxy:IdentityClientKey") ??
                    configuration.GetValue<string>("HttpClients:Standard.AspNetCore.TestApplication.Services:IdentityClientKey") ??
                    "default");

            services
                .AddHttpClient<IInvoiceServiceProxy, InvoiceServiceProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "Standard.AspNetCore.TestApplication.Services", "InvoiceServiceProxy");
                });

            services
                .AddHttpClient<IMultiVersionServiceProxy, MultiVersionServiceProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "Standard.AspNetCore.TestApplication.Services", "MultiVersionServiceProxy");
                });

            services
                .AddHttpClient<IQueryStringNamesService, QueryStringNamesServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "Standard.AspNetCore.TestApplication.Services", "QueryStringNamesService");
                });

            services
                .AddHttpClient<IVersionOneServiceProxy, VersionOneServiceProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "Standard.AspNetCore.TestApplication.Services", "VersionOneServiceProxy");
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