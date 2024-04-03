using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Eventing;
using Kafka.Consumer.Application.Common.Interfaces;
using Kafka.Consumer.Domain.Common.Interfaces;
using Kafka.Consumer.Domain.Repositories;
using Kafka.Consumer.Infrastructure.Configuration;
using Kafka.Consumer.Infrastructure.Eventing;
using Kafka.Consumer.Infrastructure.Persistence;
using Kafka.Consumer.Infrastructure.Repositories;
using Kafka.Consumer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Kafka.Consumer.Infrastructure
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