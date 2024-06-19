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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}