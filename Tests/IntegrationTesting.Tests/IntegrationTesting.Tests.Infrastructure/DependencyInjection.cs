using IntegrationTesting.Tests.Application.Common.Interfaces;
using IntegrationTesting.Tests.Domain.Common.Interfaces;
using IntegrationTesting.Tests.Domain.Repositories;
using IntegrationTesting.Tests.Infrastructure.Persistence;
using IntegrationTesting.Tests.Infrastructure.Repositories;
using IntegrationTesting.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IBadSignaturesRepository, BadSignaturesRepository>();
            services.AddTransient<IBrandRepository, BrandRepository>();
            services.AddTransient<IChildRepository, ChildRepository>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IDiffIdRepository, DiffIdRepository>();
            services.AddTransient<IDiffPkRepository, DiffPkRepository>();
            services.AddTransient<IDtoReturnRepository, DtoReturnRepository>();
            services.AddTransient<IHasDateOnlyFieldRepository, HasDateOnlyFieldRepository>();
            services.AddTransient<IHasMissingDepRepository, HasMissingDepRepository>();
            services.AddTransient<IMissingDepRepository, MissingDepRepository>();
            services.AddTransient<INoReturnRepository, NoReturnRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IParentRepository, ParentRepository>();
            services.AddTransient<IPartialCrudRepository, PartialCrudRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IRichProductRepository, RichProductRepository>();
            services.AddTransient<IUniqueConValRepository, UniqueConValRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}