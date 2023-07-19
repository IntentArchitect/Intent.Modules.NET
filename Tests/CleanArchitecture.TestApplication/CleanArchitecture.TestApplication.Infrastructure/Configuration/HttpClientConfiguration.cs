using System;
using CleanArchitecture.TestApplication.Application.IntegrationServices.SecureServicesService;
using CleanArchitecture.TestApplication.Application.IntegrationServices.TestUnversionedProxy;
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

            services.AddHttpClient<ISecureServicesClient, SecureServicesServiceHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:SecureServicesService:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:SecureServicesService:Timeout") ?? TimeSpan.FromSeconds(100);
            }).AddClientAccessTokenHandler(configuration.GetValue<string>("HttpClients:SecureServicesService:IdentityClientKey") ?? "default");
            services.AddHttpClient<ITestUnversionedProxyClient, TestUnversionedProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:TestUnversionedProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:TestUnversionedProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
            services.AddHttpClient<ITestVersionedProxyClient, TestVersionedProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:TestVersionedProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:TestVersionedProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
        }
    }
}