using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RichDomain.Application.Common.Interfaces;
using RichDomain.Domain.Common.Interfaces;
using RichDomain.Domain.Repositories;
using RichDomain.Infrastructure.Persistence;
using RichDomain.Infrastructure.Repositories;
using RichDomain.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace RichDomain.Infrastructure
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
            services.AddTransient<IBaseClassRepository, BaseClassRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IDerivedClassRepository, DerivedClassRepository>();
            services.AddTransient<IDerivedFromAbstractClassRepository, DerivedFromAbstractClassRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}