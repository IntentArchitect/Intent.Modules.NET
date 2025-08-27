using System.Reflection;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Application;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using GraphQL.MongoDb.TestApplication.Domain.Common.Interfaces;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using GraphQL.MongoDb.TestApplication.Infrastructure.Persistence;
using GraphQL.MongoDb.TestApplication.Infrastructure.Persistence.Documents;
using GraphQL.MongoDb.TestApplication.Infrastructure.Repositories;
using GraphQL.MongoDb.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(sp =>
                    {
                        var connectionString = configuration.GetConnectionString("MongoDbConnection");
                        return new MongoClient(connectionString);
                    });
            services.AddSingleton(sp =>
                    {
                        var connectionString = configuration.GetConnectionString("MongoDbConnection");

                        // Parse connection string to get the database name
                        var mongoUrl = new MongoUrl(connectionString);
                        var client = sp.GetRequiredService<IMongoClient>();

                        return client.GetDatabase(mongoUrl.DatabaseName);
                    });
            services.AddSingleton<IMongoCollection<AssignedPrivilegeDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<AssignedPrivilegeDocument>("AssignedPrivilege");
                            });
            services.AddSingleton<IMongoCollection<PrivilegeDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<PrivilegeDocument>("Privilege");
                            });
            services.AddSingleton<IMongoCollection<UserDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<UserDocument>("User");
                            });
            services.AddScoped<IPrivilegeRepository, PrivilegeMongoRepository>();
            services.AddScoped<IUserRepository, UserMongoRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<MongoDbUnitOfWork>();
            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            return services;
        }
    }
}