using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlDbProject.Application.Common.Interfaces;
using SqlDbProject.Domain.Common.Interfaces;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Domain.Repositories.Accounts;
using SqlDbProject.Infrastructure.Persistence;
using SqlDbProject.Infrastructure.Repositories;
using SqlDbProject.Infrastructure.Repositories.Accounts;
using SqlDbProject.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace SqlDbProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IShareholderRepository, ShareholderRepository>();
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<IPolicyRepository, PolicyRepository>();
            services.AddTransient<IPolicyStatusRepository, PolicyStatusRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IStakeholderRepository, StakeholderRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountTypeRepository, AccountTypeRepository>();
            services.AddTransient<ICurrencyRepository, CurrencyRepository>();
            services.AddTransient<IPeriodRepository, PeriodRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}