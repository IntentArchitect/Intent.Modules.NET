using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValueObjects.Class.Application.Common.Interfaces;
using ValueObjects.Class.Domain.Common.Interfaces;
using ValueObjects.Class.Domain.Repositories;
using ValueObjects.Class.Infrastructure.Persistence;
using ValueObjects.Class.Infrastructure.Repositories;
using ValueObjects.Class.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace ValueObjects.Class.Infrastructure
{
    public static class DependencyInjection
    {
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
            services.AddTransient<ITestEntityRepository, TestEntityRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}