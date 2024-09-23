using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.RichDomain.Tests.Infrastructure.Repositories;
using AdvancedMappingCrud.RichDomain.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Infrastructure
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
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ISuperRepository, SuperRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}