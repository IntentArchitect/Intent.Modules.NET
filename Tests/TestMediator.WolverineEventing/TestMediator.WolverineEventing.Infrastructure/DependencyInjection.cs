using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.WolverineEventing.Application.Common.Eventing;
using TestMediator.WolverineEventing.Domain.Common.Interfaces;
using TestMediator.WolverineEventing.Domain.Repositories;
using TestMediator.WolverineEventing.Infrastructure.Configuration;
using TestMediator.WolverineEventing.Infrastructure.Eventing;
using TestMediator.WolverineEventing.Infrastructure.Persistence;
using TestMediator.WolverineEventing.Infrastructure.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestMediator.WolverineEventing.Infrastructure
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
            services.AddScoped<IMessageBus, WolverineMessageBus>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddMediatorConfiguration();
            return services;
        }
    }
}