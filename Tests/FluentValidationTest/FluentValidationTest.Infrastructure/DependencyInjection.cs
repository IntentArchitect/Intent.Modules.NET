using FluentValidationTest.Domain.Common.Interfaces;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.ConstructorOperationConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.Nullability;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.NumericConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.PatternConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.TextConstraints;
using FluentValidationTest.Infrastructure.Persistence;
using FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.ConstructorOperationConstraints;
using FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.Nullability;
using FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.NumericConstraints;
using FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.PatternConstraints;
using FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.StressSuite;
using FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.TextConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace FluentValidationTest.Infrastructure
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
            services.AddTransient<IConstructedConstrainedEntityRepository, ConstructedConstrainedEntityRepository>();
            services.AddTransient<IUniqueAccountEntityRepository, UniqueAccountEntityRepository>();
            services.AddTransient<IUniquePersonEntityRepository, UniquePersonEntityRepository>();
            services.AddTransient<INullabilityConstrainedEntityRepository, NullabilityConstrainedEntityRepository>();
            services.AddTransient<INumericConstrainedEntityRepository, NumericConstrainedEntityRepository>();
            services.AddTransient<IPatternConstrainedEntityRepository, PatternConstrainedEntityRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IPersistencePrecedenceRepository, PersistencePrecedenceRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IUserAccountRepository, UserAccountRepository>();
            services.AddTransient<ITextConstrainedEntityRepository, TextConstrainedEntityRepository>();
            return services;
        }
    }
}