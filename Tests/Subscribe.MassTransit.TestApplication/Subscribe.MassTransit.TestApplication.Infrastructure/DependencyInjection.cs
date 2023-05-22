using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.TestApplication.Application.Common.Eventing;
using Subscribe.MassTransit.TestApplication.Domain.Common.Interfaces;
using Subscribe.MassTransit.TestApplication.Infrastructure.Configuration;
using Subscribe.MassTransit.TestApplication.Infrastructure.Eventing;
using Subscribe.MassTransit.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Subscribe.MassTransit.TestApplication.Infrastructure
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
            services.AddScoped<MassTransitEventBus>();
            services.AddTransient<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}