using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.Publisher.Application.Common.Interfaces;
using TestMediator.Publisher.Domain.Common.Interfaces;
using TestMediator.Publisher.Domain.Repositories;
using TestMediator.Publisher.Infrastructure.Configuration;
using TestMediator.Publisher.Infrastructure.Persistence;
using TestMediator.Publisher.Infrastructure.Repositories;
using TestMediator.Publisher.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestMediator.Publisher.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddTransient<IAuditEntryRepository, AuditEntryRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddMediatorConfiguration();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}