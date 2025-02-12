using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Repositories;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure
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
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IChildParentExcludedRepository, ChildParentExcludedRepository>();
            services.AddTransient<IChildSimpleRepository, ChildSimpleRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICustomConstructorRepository, CustomConstructorRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IFamilyComplexRepository, FamilyComplexRepository>();
            services.AddTransient<IFamilyComplexSkippedRepository, FamilyComplexSkippedRepository>();
            services.AddTransient<IFamilySimpleRepository, FamilySimpleRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IStockRepository, StockRepository>();
            services.AddTransient<ISuperRepository, SuperRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}