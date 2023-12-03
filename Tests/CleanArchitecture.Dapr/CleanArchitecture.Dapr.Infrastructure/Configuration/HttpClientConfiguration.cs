using System;
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
                    http.BaseAddress = new Uri($"http://clean-architecture-dapr");
                })
                .AddDapr();

            services
                .AddHttpClient<ISecuredService, SecuredServiceHttpClient>(http =>
                {
                    http.BaseAddress = new Uri($"http://clean-architecture-dapr");
                })
                .AddHeaders(config =>
                {
                    config.AddFromHeader("Authorization");
                })
                .AddDapr();
        }
    }
}