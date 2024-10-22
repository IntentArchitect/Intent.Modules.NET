using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Kernel.Tests.Application.Common.Interfaces;
using SharedKernel.Kernel.Tests.Domain.Common.Interfaces;
using SharedKernel.Kernel.Tests.Domain.Repositories;
using SharedKernel.Kernel.Tests.Infrastructure.Persistence;
using SharedKernel.Kernel.Tests.Infrastructure.Repositories;
using SharedKernel.Kernel.Tests.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace SharedKernel.Kernel.Tests.Infrastructure
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
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}