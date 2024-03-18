using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Eventing;
using Kafka.Producer.Application.Common.Interfaces;
using Kafka.Producer.Domain.Common.Interfaces;
using Kafka.Producer.Domain.Repositories;
using Kafka.Producer.Infrastructure.Configuration;
using Kafka.Producer.Infrastructure.Eventing;
using Kafka.Producer.Infrastructure.Persistence;
using Kafka.Producer.Infrastructure.Repositories;
using Kafka.Producer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Kafka.Producer.Infrastructure
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
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IEventBus, KafkaEventBus>();
            services.AddKafkaConfiguration();
            return services;
        }
    }
}