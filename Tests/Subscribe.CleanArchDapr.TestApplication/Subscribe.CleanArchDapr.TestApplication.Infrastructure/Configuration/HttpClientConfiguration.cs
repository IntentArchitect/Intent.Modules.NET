using System;
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
                    http.BaseAddress = new Uri($"http://publish-clean-arch-dapr-test-application");
                })
                .AddDapr();
        }
    }
}