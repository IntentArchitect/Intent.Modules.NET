using System;
using System.Net.Http;
using AzureFunctions.TestApplication.Application.IntegrationServices;
using AzureFunctions.TestApplication.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace AzureFunctions.TestApplication.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();

            services
                .AddHttpClient<ICustomersService, CustomersServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "CustomersService");
                });

            services
                .AddHttpClient<IEnumService, EnumServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "EnumService");
                });

            services
                .AddHttpClient<IIgnoresService, IgnoresServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "IgnoresService");
                });

            services
                .AddHttpClient<IListedUnlistedServicesService, ListedUnlistedServicesServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "ListedUnlistedServicesService");
                });

            services
                .AddHttpClient<INullableResultService, NullableResultServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "NullableResultService");
                });

            services
                .AddHttpClient<IParamsService, ParamsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "ParamsService");
                });

            services
                .AddHttpClient<ISampleDomainsService, SampleDomainsServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "SampleDomainsService");
                });

            services
                .AddHttpClient<IValidationService, ValidationServiceHttpClient>(http =>
                {
                    ApplyAppSettings(http, configuration, "AzureFunctions.TestApplication.Services", "ValidationService");
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