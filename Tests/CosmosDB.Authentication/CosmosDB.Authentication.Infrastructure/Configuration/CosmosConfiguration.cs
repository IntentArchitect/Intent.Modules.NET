using Azure.Identity;
using CosmosDB.Authentication.Infrastructure.Options;
using CosmosDB.Authentication.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBConfiguration", Version = "1.0")]

namespace CosmosDB.Authentication.Infrastructure.Configuration
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
                    BuildContainer(options, cosmosOptions);

                    options.CosmosConnectionString = null;
                    options.TokenCredential = credential;
                });
                return services;
            }

            services.AddCosmosRepository(options =>
            {
                BuildContainer(options, cosmosOptions);

                options.AccountEndpoint = null;
                options.TokenCredential = null;
            });

            return services;
        }

        private static void BuildContainer(RepositoryOptions options, CosmosRepositoryOptions cosmosOptions)
        {
            var defaultContainerId = cosmosOptions.ContainerId;

            if (string.IsNullOrWhiteSpace(defaultContainerId))
            {
                throw new Exception("\"RepositoryOptions:ContainerId\" configuration not specified");
            }

            options.ContainerPerItemType = true;

            options.ContainerBuilder
                .Configure<ProductDocument>(c => c
                    .WithContainer(defaultContainerId)
                    .WithPartitionKey("/name"));
        }
    }
}