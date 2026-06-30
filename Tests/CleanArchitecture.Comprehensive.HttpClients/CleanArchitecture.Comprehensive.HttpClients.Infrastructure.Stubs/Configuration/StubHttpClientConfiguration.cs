using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Stubs.HttpClients.Customers;
using CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Stubs.HttpClients.QueryDtoParameter;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.Stubs.StubHttpClientConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Stubs.Configuration
{
    public static class StubHttpClientConfiguration
    {
        public static IServiceCollection AddStubHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            if (UseStubHttpClient(configuration, "CleanArchitecture.Comprehensive.Services", "CustomersService"))
            {
                services.RemoveAll<ICustomersService>();
                services.AddTransient<ICustomersService, CustomersServiceHttpClientStub>();
            }

            if (UseStubHttpClient(configuration, "CleanArchitecture.Comprehensive.Services", "QueryDtoParameterService"))
            {
                services.RemoveAll<IQueryDtoParameterService>();
                services.AddTransient<IQueryDtoParameterService, QueryDtoParameterServiceHttpClientStub>();
            }
            return services;
        }

        private static bool UseStubHttpClient(IConfiguration configuration, string groupName, string serviceName)
        {
            return configuration.GetValue<bool?>($"HttpClients:{serviceName}:UseStub") ?? configuration.GetValue<bool?>($"HttpClients:{groupName}:UseStub") ?? false;
        }
    }
}