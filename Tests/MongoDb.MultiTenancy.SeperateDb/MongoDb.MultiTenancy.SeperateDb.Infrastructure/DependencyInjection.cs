using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.MultiTenancy.SeperateDb.Application.Common.Interfaces;
using MongoDb.MultiTenancy.SeperateDb.Domain.Common.Interfaces;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories;
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
            services.AddSingleton<MongoDbConnectionFactory>();
            services.AddScoped<IMongoDbConnection>(provider =>
                    {
                        var tenantInfo = provider.GetService<ITenantInfo>() ?? throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant info.");
                        return provider.GetRequiredService<MongoDbConnectionFactory>().GetConnection(tenantInfo);
                    });
            services.AddTransient<ICustomerRepository, CustomerMongoRepository>();
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<ApplicationMongoDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}