using System.Linq;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBMultiTenancyConfiguration", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Configuration
{
    public static class CosmosDBMultiTenancyConfiguration
    {
        public static IServiceCollection ConfigureCosmosSeperateDBMultiTenancy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ICosmosClientProvider, CosmosDBMultiTenantClientProvider>();
            services.AddSingleton(typeof(ICosmosContainerProvider<>), typeof(CosmosDBMultitenantContainerProvider<>));


            var basePostConfigureActions = services
                .Where(descriptor => descriptor.ServiceType == typeof(IPostConfigureOptions<RepositoryOptions>))
                .Select(descriptor => (IPostConfigureOptions<RepositoryOptions>)descriptor.ImplementationInstance)
                .ToList();

            services.AddSingleton<RepositoryOptions>(sp => new CosmosDBMultiTenantRepositoryOptions(sp.GetRequiredService<ICosmosClientProvider>()));
            services.AddSingleton<IOptions<RepositoryOptions>>(sp =>
            {
                var resultOptions = sp.GetRequiredService<RepositoryOptions>();
                foreach (var action in basePostConfigureActions)
                {
                    action.PostConfigure(Options.DefaultName, resultOptions);
                }
                return Options.Create(resultOptions);
            });
            services.AddSingleton<IOptionsMonitor<RepositoryOptions>>(sp => new CosmosDBMultiTenantOptionsMonitor(sp.GetRequiredService<RepositoryOptions>()));
            return services;
        }
    }
}