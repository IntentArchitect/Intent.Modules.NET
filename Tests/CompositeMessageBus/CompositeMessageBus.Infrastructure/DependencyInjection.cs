using CompositeMessageBus.Domain.Common.Interfaces;
using CompositeMessageBus.Domain.Repositories;
using CompositeMessageBus.Infrastructure.Configuration;
using CompositeMessageBus.Infrastructure.Persistence;
using CompositeMessageBus.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure
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
            services.AddScoped<DaprStateStoreUnitOfWork>();
            services.AddScoped<IDaprStateStoreUnitOfWork>(provider => provider.GetRequiredService<DaprStateStoreUnitOfWork>());
            services.AddScoped<IDaprStateStoreGenericRepository, DaprStateStoreGenericRepository>();
            services.ConfigureAws(configuration);
            services.AddHttpClients(configuration);
            services.ConfigureCompositeMessageBus(configuration);
            return services;
        }
    }
}