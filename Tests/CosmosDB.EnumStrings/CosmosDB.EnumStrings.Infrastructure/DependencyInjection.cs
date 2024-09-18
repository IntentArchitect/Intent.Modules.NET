using CosmosDB.EnumStrings.Domain.Common.Interfaces;
using CosmosDB.EnumStrings.Domain.Repositories;
using CosmosDB.EnumStrings.Infrastructure.Persistence;
using CosmosDB.EnumStrings.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CosmosDB.EnumStrings.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository();
            services.AddScoped<IRootEntityRepository, RootEntityCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            return services;
        }
    }
}