using System;
using System.Net.Http;
using CleanArchitecture.Dapr.Application.IntegrationServices;
using CleanArchitecture.Dapr.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientConfigurationTemplate", Version = "1.0")]

namespace CleanArchitecture.Dapr.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services
                .AddHttpClient<INamedQueryStringsService, NamedQueryStringsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "clean-architecture-dapr", "NamedQueryStringsService");
                })
                .ConfigureForDapr();

            services
                .AddHttpClient<ISecuredService, SecuredServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "clean-architecture-dapr", "SecuredService");
                })
                .AddHeaders(config =>
                {
                    config.AddFromHeader("Authorization");
                })
                .ConfigureForDapr();
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