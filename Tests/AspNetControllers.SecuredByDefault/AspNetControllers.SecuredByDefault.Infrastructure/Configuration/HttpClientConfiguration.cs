using AspNetControllers.SecuredByDefault.Application.IntegrationServices;
using AspNetControllers.SecuredByDefault.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace AspNetControllers.SecuredByDefault.Infrastructure.Configuration
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
                .AddHttpClient<ITestService, TestServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AspNetControllers.SecuredByDefault.Services", "TestService");
                })
                .AddClientCredentialsTokenHandler(configuration.GetValue<string>("HttpClients:TestService:IdentityClientKey") ??
                    configuration.GetValue<string>("HttpClients:AspNetControllers.SecuredByDefault.Services:IdentityClientKey") ??
                    "default");
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