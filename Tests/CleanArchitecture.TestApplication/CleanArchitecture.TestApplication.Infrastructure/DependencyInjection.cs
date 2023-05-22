using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.DDD;
using CleanArchitecture.TestApplication.Domain.Repositories.DefaultDiagram;
using CleanArchitecture.TestApplication.Infrastructure.Persistence;
using CleanArchitecture.TestApplication.Infrastructure.Repositories;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.CRUD;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.DDD;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.DefaultDiagram;
using CleanArchitecture.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure
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
            services.AddTransient<IAccountHolderRepository, AccountHolderRepository>();
            services.AddTransient<IAggregateRootRepository, AggregateRootRepository>();
            services.AddTransient<IAggregateRootLongRepository, AggregateRootLongRepository>();
            services.AddTransient<IAggregateSingleCRepository, AggregateSingleCRepository>();
            services.AddTransient<IAggregateTestNoIdReturnRepository, AggregateTestNoIdReturnRepository>();
            services.AddTransient<IClassWithDefaultRepository, ClassWithDefaultRepository>();
            services.AddTransient<IDataContractClassRepository, DataContractClassRepository>();
            services.AddTransient<IImplicitKeyAggrRootRepository, ImplicitKeyAggrRootRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}