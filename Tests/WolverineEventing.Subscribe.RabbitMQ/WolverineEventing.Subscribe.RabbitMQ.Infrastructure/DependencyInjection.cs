using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Eventing;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Common.Interfaces;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Repositories;
using WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Eventing;
using WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Persistence;
using WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Repositories;

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
            services.AddScoped<IMessageBus, WolverineMessageBus>();
            services.AddTransient<IShippedOrderRecordRepository, ShippedOrderRecordRepository>();
            return services;
        }
    }
}