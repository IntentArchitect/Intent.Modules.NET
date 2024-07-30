using System;
using System.Net.Http;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientConfigurationTemplate", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services
                .AddHttpClient<IMyProxy, MyProxyHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "publish-clean-arch-dapr-test-application", "MyProxy");
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