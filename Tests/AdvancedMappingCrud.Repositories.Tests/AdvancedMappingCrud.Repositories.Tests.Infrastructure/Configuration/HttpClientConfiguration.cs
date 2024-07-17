using System;
using System.Net.Http;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {

            services
                .AddHttpClient<ICustomersServiceProxy, CustomersServiceProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AdvancedMappingCrud.DbContext.Tests.Services", "CustomersServiceProxy");
                });

            services
                .AddHttpClient<IFileUploadsService, FileUploadsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AdvancedMappingCrud.Repositories.Tests.Services", "FileUploadsService");
                });

            services
                .AddHttpClient<IProductServiceProxy, ProductServiceProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AdvancedMappingCrud.DbContext.Tests.Services", "ProductServiceProxy");
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