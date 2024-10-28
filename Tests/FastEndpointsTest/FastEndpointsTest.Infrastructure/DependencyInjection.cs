using FastEndpointsTest.Application.Common.Interfaces;
using FastEndpointsTest.Domain.Common.Interfaces;
using FastEndpointsTest.Domain.Repositories.CRUD;
using FastEndpointsTest.Domain.Repositories.DDD;
using FastEndpointsTest.Domain.Repositories.Enums;
using FastEndpointsTest.Domain.Repositories.Pagination;
using FastEndpointsTest.Domain.Repositories.UniqueIndexConstraint;
using FastEndpointsTest.Infrastructure.Persistence;
using FastEndpointsTest.Infrastructure.Repositories.CRUD;
using FastEndpointsTest.Infrastructure.Repositories.DDD;
using FastEndpointsTest.Infrastructure.Repositories.Enums;
using FastEndpointsTest.Infrastructure.Repositories.Pagination;
using FastEndpointsTest.Infrastructure.Repositories.UniqueIndexConstraint;
using FastEndpointsTest.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace FastEndpointsTest.Infrastructure
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
            services.AddTransient<IAggregateRootRepository, AggregateRootRepository>();
            services.AddTransient<IAggregateRootLongRepository, AggregateRootLongRepository>();
            services.AddTransient<IAggregateSingleCRepository, AggregateSingleCRepository>();
            services.AddTransient<IAggregateTestNoIdReturnRepository, AggregateTestNoIdReturnRepository>();
            services.AddTransient<IAccountHolderRepository, AccountHolderRepository>();
            services.AddTransient<ICameraRepository, CameraRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IDataContractClassRepository, DataContractClassRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IClassWithEnumsRepository, ClassWithEnumsRepository>();
            services.AddTransient<ILogEntryRepository, LogEntryRepository>();
            services.AddTransient<IPersonEntryRepository, PersonEntryRepository>();
            services.AddTransient<IAggregateWithUniqueConstraintIndexElementRepository, AggregateWithUniqueConstraintIndexElementRepository>();
            services.AddTransient<IAggregateWithUniqueConstraintIndexStereotypeRepository, AggregateWithUniqueConstraintIndexStereotypeRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}