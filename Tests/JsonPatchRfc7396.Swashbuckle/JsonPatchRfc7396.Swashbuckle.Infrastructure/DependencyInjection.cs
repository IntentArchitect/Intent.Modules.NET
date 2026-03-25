using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Common.Interfaces;
using JsonPatchRfc7396.Swashbuckle.Domain.Repositories.CollaborativeEditing;
using JsonPatchRfc7396.Swashbuckle.Domain.Repositories.Configuration;
using JsonPatchRfc7396.Swashbuckle.Infrastructure.Configuration;
using JsonPatchRfc7396.Swashbuckle.Infrastructure.Persistence;
using JsonPatchRfc7396.Swashbuckle.Infrastructure.Repositories.CollaborativeEditing;
using JsonPatchRfc7396.Swashbuckle.Infrastructure.Repositories.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            var cs = configuration.GetConnectionString("MongoDbConnection");
            services.TryAddSingleton<IMongoClient>(_ => new MongoClient(cs));
            services.TryAddSingleton<IMongoDatabase>(sp =>
                    {
                        var dbName = new MongoUrl(cs).DatabaseName
                                     ?? throw new InvalidOperationException(
                                         "MongoDbConnection must include a database name.");
                        return sp.GetRequiredService<IMongoClient>().GetDatabase(dbName);
                    });
            services.RegisterMongoCollections(typeof(DependencyInjection).Assembly);
            services.AddScoped<IDocumentRepository, DocumentMongoRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IConfigurationStoreRepository, ConfigurationStoreRepository>();
            services.AddScoped<MongoDbUnitOfWork>();
            services.AddScoped<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<MongoDbUnitOfWork>());
            return services;
        }
    }
}