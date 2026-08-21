using Dapper.Tests.Application.Common.Interfaces;
using Dapper.Tests.Domain.Repositories;
using Dapper.Tests.Infrastructure.Repositories;
using Dapper.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Dapper.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAuditLogEntryRepository, AuditLogEntryRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IDualGeneratedKeyEntityRepository, DualGeneratedKeyEntityRepository>();
            services.AddTransient<IMixedKeyEntityRepository, MixedKeyEntityRepository>();
            services.AddTransient<IOrderLineRepository, OrderLineRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}
