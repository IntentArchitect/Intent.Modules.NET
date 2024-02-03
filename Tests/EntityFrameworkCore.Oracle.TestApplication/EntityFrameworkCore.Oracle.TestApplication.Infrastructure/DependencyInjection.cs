using EntityFrameworkCore.Oracle.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.Oracle.TestApplication.Domain.Common.Interfaces;
using EntityFrameworkCore.Oracle.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Oracle.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.Oracle.TestApplication.Infrastructure.Repositories;
using EntityFrameworkCore.Oracle.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.Oracle.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseOracle(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}