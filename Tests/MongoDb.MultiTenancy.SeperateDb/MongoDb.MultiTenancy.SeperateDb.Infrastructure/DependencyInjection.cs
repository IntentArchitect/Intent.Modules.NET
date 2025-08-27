using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDb.MultiTenancy.SeperateDb.Application.Common.Interfaces;
using MongoDb.MultiTenancy.SeperateDb.Domain.Common.Interfaces;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.MultiTenant;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence.Documents;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Repositories;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITenantConnections>(
                provider => provider.GetService<ITenantInfo>() as TenantExtendedInfo ??
                throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant info"));
            services.AddSingleton<MongoDbMultiTenantConnectionFactory>();
            services.AddScoped<IMongoDatabase>(provider =>
                    {
                        var tenantConnections = provider.GetService<ITenantConnections>();
                        if (tenantConnections is null || tenantConnections.MongoDbConnection is null)
                        {
                            throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant MongoDb connection information");
                        }
                        return provider.GetRequiredService<MongoDbMultiTenantConnectionFactory>().GetConnection(tenantConnections.MongoDbConnection);
                    });
            services.AddScoped<IMongoCollection<CustomerDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<CustomerDocument>("Customer");
                            });
            services.AddScoped<ICustomerRepository, CustomerMongoRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<MongoDbUnitOfWork>();
            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            return services;
        }
    }
}