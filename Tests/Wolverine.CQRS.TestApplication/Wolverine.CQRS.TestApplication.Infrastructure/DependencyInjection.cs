using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Domain.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;
using Wolverine.CQRS.TestApplication.Infrastructure.Dispatch.Middleware;
using Wolverine.CQRS.TestApplication.Infrastructure.Persistence;
using Wolverine.CQRS.TestApplication.Infrastructure.Repositories.Items;
using Wolverine.CQRS.TestApplication.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<AuthorizationMiddleware>();
            services.AddTransient<ValidationMiddleware>();
            services.AddTransient<LoggingMiddleware>();
            services.AddTransient<PerformanceMiddleware>();
            services.AddTransient<UnhandledExceptionMiddleware>();
            services.AddTransient<UnitOfWorkMiddleware>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddTransient<IItemRepository, ItemRepository>();
            return services;
        }
    }
}
