using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using GraphQL.MongoDb.TestApplication.Domain.Common.Interfaces;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using GraphQL.MongoDb.TestApplication.Infrastructure.Persistence;
using GraphQL.MongoDb.TestApplication.Infrastructure.Repositories;
using GraphQL.MongoDb.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetService<ApplicationMongoDbContext>());
            services.AddTransient<IPrivilegeRepository, PrivilegeMongoRepository>();
            services.AddTransient<IUserRepository, UserMongoRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}