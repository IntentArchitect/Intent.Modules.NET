using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.IntegrationServiceProxy;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.InvoiceServiceProxy;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.MultiVersionServiceProxy;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.VersionOneServiceProxy;
using Standard.AspNetCore.TestApplication.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();

            services.AddHttpClient<IIntegrationServiceProxyClient, IntegrationServiceProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:IntegrationServiceProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:IntegrationServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
            services.AddHttpClient<IInvoiceServiceProxyClient, InvoiceServiceProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:InvoiceServiceProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:InvoiceServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
            services.AddHttpClient<IMultiVersionServiceProxyClient, MultiVersionServiceProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:MultiVersionServiceProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:MultiVersionServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
            services.AddHttpClient<IVersionOneServiceProxyClient, VersionOneServiceProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:VersionOneServiceProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:VersionOneServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
        }
    }
}