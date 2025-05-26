using System;
using System.Net.Http;
using AspNetCore.Controllers.Secured.Application.IntegrationServices;
using AspNetCore.Controllers.Secured.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var clientCredentialsBuilder = services.AddClientCredentialsTokenManagement();
            foreach (var clientCredentials in configuration.GetSection("IdentityClients").GetChildren())
            {
                clientCredentialsBuilder.AddClient(clientCredentials.Key, clientCredentials.Bind);
            }

            services
                .AddHttpClient<IBuyersService, BuyersServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "PackageLevelSecurity", "BuyersService");
                })
                .AddClientCredentialsTokenHandler(configuration.GetValue<string>("HttpClients:BuyersService:IdentityClientKey") ??
                    configuration.GetValue<string>("HttpClients:PackageLevelSecurity:IdentityClientKey") ??
                    "default");

            services
                .AddHttpClient<IProductsService, ProductsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AspNetCore.Controllers.Secured.Services", "ProductsService");
                })
                .AddClientCredentialsTokenHandler(configuration.GetValue<string>("HttpClients:ProductsService:IdentityClientKey") ??
                    configuration.GetValue<string>("HttpClients:AspNetCore.Controllers.Secured.Services:IdentityClientKey") ??
                    "default");
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