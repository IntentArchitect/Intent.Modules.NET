using System;
using CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy;
using CleanArchitecture.TestApplication.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "1.0")]

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

            services.AddHttpClient<ITestVersionedProxyClient, TestVersionedProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:TestVersionedProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:TestVersionedProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
        }
    }
}