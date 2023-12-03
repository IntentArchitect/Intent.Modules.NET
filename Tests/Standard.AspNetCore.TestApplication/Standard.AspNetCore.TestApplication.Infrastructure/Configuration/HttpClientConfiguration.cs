using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices;
using Standard.AspNetCore.TestApplication.Infrastructure.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

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
            services.AddHttpContextAccessor();

            services
                .AddHttpClient<IIntegrationServiceProxy, IntegrationServiceProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:IntegrationServiceProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:IntegrationServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                })
                .AddClientAccessTokenHandler(configuration.GetValue<string>("HttpClients:IntegrationServiceProxy:IdentityClientKey") ?? "default");

            services
                .AddHttpClient<IInvoiceServiceProxy, InvoiceServiceProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:InvoiceServiceProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:InvoiceServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IMultiVersionServiceProxy, MultiVersionServiceProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:MultiVersionServiceProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:MultiVersionServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IQueryStringNamesService, QueryStringNamesServiceHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:QueryStringNamesService:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:QueryStringNamesService:Timeout") ?? TimeSpan.FromSeconds(100);
                });

            services
                .AddHttpClient<IVersionOneServiceProxy, VersionOneServiceProxyHttpClient>(http =>
                {
                    http.BaseAddress = configuration.GetValue<Uri>("HttpClients:VersionOneServiceProxy:Uri");
                    http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:VersionOneServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
                });
        }
    }
}