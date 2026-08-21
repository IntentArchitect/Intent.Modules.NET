using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Publish.RabbitMQ.Domain.Common.Interfaces;
using Wolverine.Publish.RabbitMQ.Infrastructure.Eventing;
using Wolverine.Publish.RabbitMQ.Infrastructure.Persistence;
using ContractsMessageBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Infrastructure
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
            services.AddScoped<ContractsMessageBus, WolverineMessageBus>();
            return services;
        }
    }
}
