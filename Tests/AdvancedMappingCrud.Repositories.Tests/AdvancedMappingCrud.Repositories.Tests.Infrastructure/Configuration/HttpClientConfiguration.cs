using System;
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
            services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();

            services
                .AddHttpClient<ICustomersServiceProxy, CustomersServiceProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:CustomersServiceProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:CustomersServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IFileUploadsService, FileUploadsServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:FileUploadsService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:FileUploadsService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IProductServiceProxy, ProductServiceProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:ProductServiceProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:ProductServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                });
        }
    }
}