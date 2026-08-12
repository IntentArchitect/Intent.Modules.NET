using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAndWorker.Domain.Common.Interfaces;
using WebAndWorker.Infrastructure.Configuration;
using WebAndWorker.Infrastructure.Persistence;
using WebAndWorker.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace WebAndWorker.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IScopedExecutor, ScopedExecutor>();
            services.AddScoped<IScopedMediator, ScopedMediator>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}