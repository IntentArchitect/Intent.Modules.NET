using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValueObjects.Record.Application.Common.Interfaces;
using ValueObjects.Record.Domain.Common.Interfaces;
using ValueObjects.Record.Domain.Repositories;
using ValueObjects.Record.Infrastructure.Persistence;
using ValueObjects.Record.Infrastructure.Repositories;
using ValueObjects.Record.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace ValueObjects.Record.Infrastructure
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