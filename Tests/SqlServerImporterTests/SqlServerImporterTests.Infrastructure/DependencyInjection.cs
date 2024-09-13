using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlServerImporterTests.Application.Common.Interfaces;
using SqlServerImporterTests.Domain.Common.Interfaces;
using SqlServerImporterTests.Domain.Repositories.Dbo;
using SqlServerImporterTests.Domain.Repositories.Schema2;
using SqlServerImporterTests.Domain.Repositories.Views;
using SqlServerImporterTests.Infrastructure.Persistence;
using SqlServerImporterTests.Infrastructure.Repositories.Dbo;
using SqlServerImporterTests.Infrastructure.Repositories.Schema2;
using SqlServerImporterTests.Infrastructure.Repositories.Views;
using SqlServerImporterTests.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure
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
            services.AddTransient<IOperationRepository, OperationRepository>();
            services.AddTransient<IStoredProcedureRepository, StoredProcedureRepository>();
            services.AddTransient<IAddressRepository, AddressRepository>();
            services.AddTransient<IAspNetRoleRepository, AspNetRoleRepository>();
            services.AddTransient<IAspNetRoleClaimRepository, AspNetRoleClaimRepository>();
            services.AddTransient<IAspNetUserRepository, AspNetUserRepository>();
            services.AddTransient<IAspNetUserClaimRepository, AspNetUserClaimRepository>();
            services.AddTransient<IAspNetUserLoginRepository, AspNetUserLoginRepository>();
            services.AddTransient<IAspNetUserRoleRepository, AspNetUserRoleRepository>();
            services.AddTransient<IAspNetUserTokenRepository, AspNetUserTokenRepository>();
            services.AddTransient<IBrandRepository, BrandRepository>();
            services.AddTransient<IChildRepository, ChildRepository>();
            services.AddTransient<Domain.Repositories.Dbo.ICustomerRepository, Repositories.Dbo.CustomerRepository>();
            services.AddTransient<ILegacyTableRepository, LegacyTableRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderItemRepository, OrderItemRepository>();
            services.AddTransient<IParentRepository, ParentRepository>();
            services.AddTransient<IPriceRepository, PriceRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IBankRepository, BankRepository>();
            services.AddTransient<IBanksRepository, BanksRepository>();
            services.AddTransient<Domain.Repositories.Schema2.ICustomerRepository, Repositories.Schema2.CustomerRepository>();
            services.AddTransient<ICustomersRepository, CustomersRepository>();
            services.AddTransient<IVwOrderRepository, VwOrderRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}