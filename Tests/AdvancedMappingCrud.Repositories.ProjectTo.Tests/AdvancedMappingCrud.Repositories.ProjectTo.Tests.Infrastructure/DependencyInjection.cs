using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Infrastructure.Repositories;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Infrastructure
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
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}