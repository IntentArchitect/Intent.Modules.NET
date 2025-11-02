using CleanArchitecture.QueueStorage.Domain.Common.Interfaces;
using CleanArchitecture.QueueStorage.Domain.Repositories;
using CleanArchitecture.QueueStorage.Infrastructure.Configuration;
using CleanArchitecture.QueueStorage.Infrastructure.Persistence;
using CleanArchitecture.QueueStorage.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure
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
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddQueueStorageConfiguration(configuration);
            return services;
        }
    }
}