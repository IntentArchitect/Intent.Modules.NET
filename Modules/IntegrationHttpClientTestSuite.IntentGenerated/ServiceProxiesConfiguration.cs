using System;
using IntegrationHttpClientTestSuite.IntentGenerated.Contracts;
using IntegrationHttpClientTestSuite.IntentGenerated.Proxies;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClient.ServiceProxiesConfiguration", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated
{
    public static class ServiceProxiesConfiguration
    {
        public static void AddServiceProxies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();

            services.AddHttpClient<IInvoiceService, InvoiceProxy>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("Proxies:InvoiceService:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("Proxies:InvoiceService:Timeout") ?? TimeSpan.FromSeconds(100);
            }).AddClientAccessTokenHandler(configuration.GetValue<string>("Proxies:InvoiceService:IdentityClientKey") ?? "default");
        }
    }
}