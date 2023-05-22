using Entities.PrivateSetters.MongoDb.Domain.Common.Interfaces;
using Entities.PrivateSetters.MongoDb.Domain.Repositories;
using Entities.PrivateSetters.MongoDb.Infrastructure.Persistence;
using Entities.PrivateSetters.MongoDb.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ApplicationMongoDbContext>();
            services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetRequiredService<ApplicationMongoDbContext>());
            services.AddTransient<IInvoiceRepository, InvoiceMongoRepository>();
            services.AddTransient<ITagRepository, TagMongoRepository>();
            return services;
        }
    }
}