using System.Reflection;
using AutoMapper;
using Entities.PrivateSetters.MongoDb.Application;
using Entities.PrivateSetters.MongoDb.Domain.Common.Interfaces;
using Entities.PrivateSetters.MongoDb.Domain.Repositories;
using Entities.PrivateSetters.MongoDb.Infrastructure.Persistence;
using Entities.PrivateSetters.MongoDb.Infrastructure.Persistence.Documents;
using Entities.PrivateSetters.MongoDb.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure
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
            services.AddSingleton<IMongoCollection<InvoiceDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<InvoiceDocument>("Invoice");
                            });
            services.AddSingleton<IMongoCollection<LineDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<LineDocument>("Line");
                            });
            services.AddSingleton<IMongoCollection<TagDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<TagDocument>("Tag");
                            });
            services.AddScoped<IInvoiceRepository, InvoiceMongoRepository>();
            services.AddScoped<ITagRepository, TagMongoRepository>();
            services.AddScoped<MongoDbUnitOfWork>();
            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            return services;
        }
    }
}