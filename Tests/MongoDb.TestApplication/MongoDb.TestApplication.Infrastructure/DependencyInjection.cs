using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Infrastructure;
using MongoDb.TestApplication.Domain.Common.Interfaces;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Repositories;
using MongoDB.UnitOfWork.Abstractions.Extensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure
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
            services.AddScoped<ApplicationMongoDbContext>(
                provider =>
                {
                    var connection = MongoDbConnection.FromConnectionString(configuration.GetConnectionString("MongoDbConnection"));
                    return new ApplicationMongoDbContext(connection);
                });
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetService<ApplicationMongoDbContext>());
            services.AddTransient<IMongoDbContext>(provider => provider.GetService<ApplicationMongoDbContext>());
            services.AddTransient<IIdGuidRepository, IdGuidRepository>();
            services.AddTransient<IIdIntRepository, IdIntRepository>();
            services.AddTransient<IIdLongRepository, IdLongRepository>();
            services.AddTransient<IIdObjectIdRepository, IdObjectIdRepository>();
            services.AddMongoDbUnitOfWork();
            services.AddMongoDbUnitOfWork<ApplicationMongoDbContext>();
            return services;
        }
    }
}