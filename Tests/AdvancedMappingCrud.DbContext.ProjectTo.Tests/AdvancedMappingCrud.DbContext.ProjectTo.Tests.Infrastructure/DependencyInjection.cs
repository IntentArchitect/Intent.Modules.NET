using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Infrastructure
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
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}