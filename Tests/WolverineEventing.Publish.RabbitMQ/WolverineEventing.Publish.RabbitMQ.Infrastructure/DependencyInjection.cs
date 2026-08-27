using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Eventing;
using WolverineEventing.Publish.RabbitMQ.Domain.Common.Interfaces;
using WolverineEventing.Publish.RabbitMQ.Infrastructure.Eventing;
using WolverineEventing.Publish.RabbitMQ.Infrastructure.Persistence;
using ContractsMessageBus = WolverineEventing.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Infrastructure
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
            return services;
        }
    }
}