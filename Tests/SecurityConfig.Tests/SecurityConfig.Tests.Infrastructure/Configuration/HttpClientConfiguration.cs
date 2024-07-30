using System;
using System.Net.Http;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecurityConfig.Tests.Application.IntegrationServices;
using SecurityConfig.Tests.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace SecurityConfig.Tests.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<ICustomersService, CustomersServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "SecurityConfig.Tests.Services", "CustomersService");
                });

            services
                .AddHttpClient<IProductsService, ProductsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "SecurityConfig.Tests.Services", "ProductsService");
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