using System.Reflection;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Domain.Common.Interfaces;
using CleanArchitecture.Comprehensive.Domain.Repositories.Async;
using CleanArchitecture.Comprehensive.Domain.Repositories.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Domain.Repositories.BugFixes;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using CleanArchitecture.Comprehensive.Domain.Repositories.ConventionBasedEventPublishing;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DefaultDiagram;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using CleanArchitecture.Comprehensive.Domain.Repositories.General;
using CleanArchitecture.Comprehensive.Domain.Repositories.Geometry;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using CleanArchitecture.Comprehensive.Domain.Repositories.Nullability;
using CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery;
using CleanArchitecture.Comprehensive.Domain.Repositories.OperationAndConstructorMapping;
using CleanArchitecture.Comprehensive.Domain.Repositories.Operations;
using CleanArchitecture.Comprehensive.Domain.Repositories.Pagination;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Infrastructure.Configuration;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.Async;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.BugFixes;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.CompositeKeys;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.ConventionBasedEventPublishing;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.CRUD;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.DDD;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.DefaultDiagram;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.Enums;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.General;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.Geometry;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.Inheritance;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.Nullability;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.ODataQuery;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.OperationAndConstructorMapping;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.Operations;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.Pagination;
using CleanArchitecture.Comprehensive.Infrastructure.Repositories.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure
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
            services.AddTransient<IAsyncOperationsClassRepository, AsyncOperationsClassRepository>();
            services.AddTransient<ISubmissionRepository, SubmissionRepository>();
            services.AddTransient<IBankRepository, BankRepository>();
            services.AddTransient<IBanksRepository, BanksRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<IWithCompositeKeyRepository, WithCompositeKeyRepository>();
            services.AddTransient<IIntegrationTriggeringRepository, IntegrationTriggeringRepository>();
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
            services.AddTransient<IClassWithDefaultRepository, ClassWithDefaultRepository>();
            services.AddTransient<IClassWithEnumsRepository, ClassWithEnumsRepository>();
            services.AddTransient<ICustomMappingRepository, CustomMappingRepository>();
            services.AddTransient<IGeometryTypeRepository, GeometryTypeRepository>();
            services.AddTransient<IBaseClassRepository, BaseClassRepository>();
            services.AddTransient<IConcreteClassRepository, ConcreteClassRepository>();
            services.AddTransient<INullabilityPeerRepository, NullabilityPeerRepository>();
            services.AddTransient<ITestNullablityRepository, TestNullablityRepository>();
            services.AddTransient<IODataAggRepository, ODataAggRepository>();
            services.AddTransient<IOpAndCtorMapping2Repository, OpAndCtorMapping2Repository>();
            services.AddTransient<IOpAndCtorMapping3Repository, OpAndCtorMapping3Repository>();
            services.AddTransient<IOperationsClassRepository, OperationsClassRepository>();
            services.AddTransient<ILogEntryRepository, LogEntryRepository>();
            services.AddTransient<IPersonEntryRepository, PersonEntryRepository>();
            services.AddTransient<IAggregateWithUniqueConstraintIndexElementRepository, AggregateWithUniqueConstraintIndexElementRepository>();
            services.AddTransient<IAggregateWithUniqueConstraintIndexStereotypeRepository, AggregateWithUniqueConstraintIndexStereotypeRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            services.AddHttpClients(configuration);
            return services;
        }
    }
}