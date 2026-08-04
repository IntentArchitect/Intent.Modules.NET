using Azure.Identity;
using AzureIdentityManagement.Infrastructure.Options;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBConfiguration", Version = "1.0")]

namespace AzureIdentityManagement.Infrastructure.Configuration
{
    public static class CosmosConfiguration
    {
        public static IServiceCollection ConfigureCosmosRepository(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<CosmosRepositoryOptions>(configuration.GetSection("RepositoryOptions"));
            var cosmosOptions = configuration.GetSection("RepositoryOptions").Get<CosmosRepositoryOptions>();

            if (cosmosOptions?.AuthenticationMethod?.ToLower() == "managedidentity")
            {
                var managedIdentityClientId = cosmosOptions?.ManagedIdentityClientId;
                var credential = string.IsNullOrWhiteSpace(managedIdentityClientId)
                ? new DefaultAzureCredential()
                : new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = managedIdentityClientId
                });

                services.AddCosmosRepository(options =>
                {
                    options.CosmosConnectionString = null;
                    options.TokenCredential = credential;
                });
                return services;
            }

            services.AddCosmosRepository(options =>
            {
                options.AccountEndpoint = null;
                options.TokenCredential = null;
            });

            return services;
        }
    }
}