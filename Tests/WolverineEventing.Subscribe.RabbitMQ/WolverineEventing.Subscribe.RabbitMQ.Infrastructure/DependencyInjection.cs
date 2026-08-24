using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Common.Interfaces;
using WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Infrastructure
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
            return services;
        }
    }
}