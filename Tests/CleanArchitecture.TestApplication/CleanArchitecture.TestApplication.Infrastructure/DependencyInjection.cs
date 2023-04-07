using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Infrastructure.Persistence;
using CleanArchitecture.TestApplication.Infrastructure.Repositories;
using CleanArchitecture.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure
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
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<IAggregateRootRepository, AggregateRootRepository>();
            services.AddTransient<IAggregateRootLongRepository, AggregateRootLongRepository>();
            services.AddTransient<IAggregateSingleCRepository, AggregateSingleCRepository>();
            services.AddTransient<IAggregateTestNoIdReturnRepository, AggregateTestNoIdReturnRepository>();
            services.AddTransient<IEntityWithCtorRepository, EntityWithCtorRepository>();
            services.AddTransient<IEntityWithMutableOperationRepository, EntityWithMutableOperationRepository>();
            services.AddTransient<IImplicitKeyAggrRootRepository, ImplicitKeyAggrRootRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}