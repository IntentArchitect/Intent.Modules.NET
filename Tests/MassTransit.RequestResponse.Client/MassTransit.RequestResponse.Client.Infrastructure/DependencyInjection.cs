using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.IntegrationServices;
using MassTransit.RequestResponse.Client.Domain.Common.Interfaces;
using MassTransit.RequestResponse.Client.Infrastructure.Configuration;
using MassTransit.RequestResponse.Client.Infrastructure.Eventing;
using MassTransit.RequestResponse.Client.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RequestResponse.Client.Infrastructure
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
            services.AddTransient<IAzureServiceBusCQRSService, AzureServiceBusCQRSService>();
            services.AddTransient<IRabbitMqCQRSService, RabbitMqCQRSService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}