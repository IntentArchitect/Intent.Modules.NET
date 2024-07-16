using System;
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
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:CustomersService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:CustomersService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IProductsService, ProductsServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:ProductsService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:ProductsService:Timeout") ?? TimeSpan.FromSeconds(100);
                });
        }
    }
}