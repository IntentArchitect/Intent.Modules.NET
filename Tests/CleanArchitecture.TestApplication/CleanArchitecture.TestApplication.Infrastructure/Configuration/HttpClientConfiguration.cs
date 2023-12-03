using System;
using CleanArchitecture.TestApplication.Application.IntegrationServices;
using CleanArchitecture.TestApplication.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();
            services.AddHttpContextAccessor();

            services
                .AddHttpClient<INamedQueryStringsService, NamedQueryStringsServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:NamedQueryStringsService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:NamedQueryStringsService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IPaginationForProxiesService, PaginationForProxiesServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:PaginationForProxiesService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:PaginationForProxiesService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IQueryDtoParameterService, QueryDtoParameterServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:QueryDtoParameterService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:QueryDtoParameterService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<ISecureServicesService, SecureServicesServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:SecureServicesService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:SecureServicesService:Timeout") ?? TimeSpan.FromSeconds(100);
                })
                .AddHeaders(config =>
                {
                    config.AddFromHeader("Authorization");
                })
                .AddClientAccessTokenHandler(configuration.GetValue<string>("HttpClients:SecureServicesService:IdentityClientKey") ?? "default");

            services
                .AddHttpClient<ITestUnversionedProxy, TestUnversionedProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:TestUnversionedProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:TestUnversionedProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<ITestVersionedProxy, TestVersionedProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:TestVersionedProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:TestVersionedProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                });
        }
    }
}