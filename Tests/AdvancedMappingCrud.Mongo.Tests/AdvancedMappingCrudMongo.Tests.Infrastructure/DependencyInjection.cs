using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Repositories;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));
            services.AddTransient<ICustomerRepository, CustomerMongoRepository>();
            services.AddTransient<IExternalDocRepository, ExternalDocMongoRepository>();
            services.AddTransient<IOrderRepository, OrderMongoRepository>();
            services.AddTransient<IProductRepository, ProductMongoRepository>();
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<ApplicationMongoDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}