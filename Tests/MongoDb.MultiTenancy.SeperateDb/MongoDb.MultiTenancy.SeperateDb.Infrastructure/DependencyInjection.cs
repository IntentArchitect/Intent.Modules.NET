using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.MultiTenancy.SeperateDb.Application.Common.Interfaces;
using MongoDb.MultiTenancy.SeperateDb.Domain.Common.Interfaces;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.MultiTenant;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Repositories;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Services;
using MongoFramework;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<MongoDbMultiTenantConnectionFactory>();
            services.AddScoped<IMongoDbConnection>(provider =>
                    {
                        var tenantConnections = provider.GetService<ITenantConnections>();
                        if (tenantConnections is null || tenantConnections.MongoDbConnection is null)
                        {
                            throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant MongoDb connection information");
                        }
                        return provider.GetRequiredService<MongoDbMultiTenantConnectionFactory>().GetConnection(tenantConnections.MongoDbConnection);
                    });
            services.AddScoped<ITenantConnections>(
                provider => provider.GetService<ITenantInfo>() as TenantExtendedInfo ??
                throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant info"));
            services.AddTransient<ICustomerRepository, CustomerMongoRepository>();
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<ApplicationMongoDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}