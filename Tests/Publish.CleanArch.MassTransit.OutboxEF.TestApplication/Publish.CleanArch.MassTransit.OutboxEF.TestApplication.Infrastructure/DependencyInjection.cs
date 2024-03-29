using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Common.Interfaces;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Common.Interfaces;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Repositories;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Infrastructure.Configuration;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Infrastructure.Eventing;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Infrastructure.Persistence;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Infrastructure.Repositories;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Infrastructure
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
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}