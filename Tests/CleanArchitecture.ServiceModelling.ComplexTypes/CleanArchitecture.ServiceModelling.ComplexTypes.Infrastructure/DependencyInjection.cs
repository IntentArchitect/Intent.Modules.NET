using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common.Interfaces;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Persistence;
using CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Repositories;
using CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure
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
            services.AddTransient<ICustomerAnemicRepository, CustomerAnemicRepository>();
            services.AddTransient<ICustomerRichRepository, CustomerRichRepository>();
            services.AddTransient<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}