using Intent.Modules.NET.Tests.Module2.Application.Interfaces;
using Intent.Modules.NET.Tests.Module2.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.Modules.NET.Tests.Module2.Infrastructure.Persistence;
using Intent.Modules.NET.Tests.Module2.Infrastructure.Repositories;
using Intent.Modules.NET.Tests.Module2.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Module2DbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("Module2");
                options.UseLazyLoadingProxies();
            });
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IMyCustomerRepository, MyCustomerRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Module2DbContext>());
            return services;
        }
    }
}