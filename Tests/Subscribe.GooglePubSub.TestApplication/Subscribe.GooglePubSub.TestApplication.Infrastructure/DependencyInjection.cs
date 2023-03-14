using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Infrastructure;
using MongoDB.UnitOfWork.Abstractions.Extensions;
using Subscribe.GooglePubSub.TestApplication.Domain.Common.Interfaces;
using Subscribe.GooglePubSub.TestApplication.Infrastructure.Configuration;
using Subscribe.GooglePubSub.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Infrastructure
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
                    var connectionString = configuration.GetConnectionString("MongoDbConnection");
                    var url = MongoDB.Driver.MongoUrl.Create(connectionString);
                    return new ApplicationMongoDbContext(connectionString, url.DatabaseName);
                });
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<IMongoDbUnitOfWork>(provider => provider.GetService<ApplicationMongoDbContext>());
            services.AddTransient<IMongoDbContext>(provider => provider.GetService<ApplicationMongoDbContext>());
            services.AddMongoDbUnitOfWork();
            services.AddMongoDbUnitOfWork<ApplicationMongoDbContext>();
            services.RegisterGoogleCloudPubSubServices(configuration);
            services.AddSubscribers();
            services.RegisterEventHandlers();
            services.RegisterTopicEvents();
            return services;
        }
    }
}