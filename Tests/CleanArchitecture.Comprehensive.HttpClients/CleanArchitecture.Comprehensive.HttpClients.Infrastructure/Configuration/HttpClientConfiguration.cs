using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Infrastructure.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var clientCredentialsBuilder = services.AddClientCredentialsTokenManagement();
            foreach (var clientCredentials in configuration.GetSection("IdentityClients").GetChildren())
            {
                clientCredentialsBuilder.AddClient(clientCredentials.Key, clientCredentials.Bind);
            }

            services
                .AddHttpClient<ICustomersService, CustomersServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "CleanArchitecture.Comprehensive.Services", "CustomersService");
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