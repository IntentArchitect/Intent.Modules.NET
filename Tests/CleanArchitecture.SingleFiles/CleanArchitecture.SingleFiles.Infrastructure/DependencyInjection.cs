using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using CleanArchitecture.SingleFiles.Infrastructure.Configuration;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents;
using CleanArchitecture.SingleFiles.Infrastructure.Repositories;
using CleanArchitecture.SingleFiles.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
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
            services.AddSingleton<IMongoCollection<MongoInvoiceDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MongoInvoiceDocument>("MongoInvoice");
                            });
            services.AddSingleton<IMongoCollection<MongoLineDocument>>(sp =>
                            {
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<MongoLineDocument>("MongoLine");
                            });
            services.AddScoped<ICosmosInvoiceRepository, CosmosInvoiceCosmosDBRepository>();
            services.AddScoped<IDaprInvoiceRepository, DaprInvoiceDaprStateStoreRepository>();
            services.AddScoped<IMongoInvoiceRepository, MongoInvoiceMongoRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<DaprStateStoreUnitOfWork>();
            services.AddScoped<IDaprStateStoreUnitOfWork>(provider => provider.GetRequiredService<DaprStateStoreUnitOfWork>());
            services.AddTransient<IEfInvoiceRepository, EfInvoiceRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<MongoDbUnitOfWork>();
            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            services.AddScoped<IDaprStateStoreGenericRepository, DaprStateStoreGenericRepository>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}