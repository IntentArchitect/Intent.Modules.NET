using System;
using System.Net.Http;
using CleanArchitecture.Comprehensive.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Configuration
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
                .AddHttpClient<IBugFixesService, BugFixesServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "BugFixesService");
                });

            services
                .AddHttpClient<INamedQueryStringsService, NamedQueryStringsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "NamedQueryStringsService");
                });

            services
                .AddHttpClient<IPaginationForProxiesService, PaginationForProxiesServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "PaginationForProxiesService");
                });

            services
                .AddHttpClient<IParamConversionService, ParamConversionServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "ParamConversionService");
                });

            services
                .AddHttpClient<IQueryDtoParameterService, QueryDtoParameterServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "QueryDtoParameterService");
                });

            services
                .AddHttpClient<ISecureServicesService, SecureServicesServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "SecureServicesService");
                })
                .AddClientAccessTokenHandler(configuration.GetValue<string>("HttpClients:SecureServicesService:IdentityClientKey") ??
                    configuration.GetValue<string>("HttpClients:CleanArchitecture.Comprehensive.Services:IdentityClientKey") ??
                    "default");

            services
                .AddHttpClient<ITestUnversionedProxy, TestUnversionedProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "TestUnversionedProxy");
                });

            services
                .AddHttpClient<ITestVersionedProxy, TestVersionedProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "TestVersionedProxy");
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