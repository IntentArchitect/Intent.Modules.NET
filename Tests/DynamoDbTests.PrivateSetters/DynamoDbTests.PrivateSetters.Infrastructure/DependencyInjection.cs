using DynamoDbTests.PrivateSetters.Application.Common.Interfaces;
using DynamoDbTests.PrivateSetters.Domain.Common.Interfaces;
using DynamoDbTests.PrivateSetters.Domain.Repositories;
using DynamoDbTests.PrivateSetters.Domain.Repositories.Folder;
using DynamoDbTests.PrivateSetters.Domain.Repositories.Throughput;
using DynamoDbTests.PrivateSetters.Infrastructure.Configuration;
using DynamoDbTests.PrivateSetters.Infrastructure.Persistence;
using DynamoDbTests.PrivateSetters.Infrastructure.Repositories;
using DynamoDbTests.PrivateSetters.Infrastructure.Repositories.Folder;
using DynamoDbTests.PrivateSetters.Infrastructure.Repositories.Throughput;
using DynamoDbTests.PrivateSetters.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBaseTypeRepository, BaseTypeDynamoDBRepository>();
            services.AddScoped<ICategoryRepository, CategoryDynamoDBRepository>();
            services.AddScoped<IClassContainerRepository, ClassContainerDynamoDBRepository>();
            services.AddScoped<IClientRepository, ClientDynamoDBRepository>();
            services.AddScoped<ICustomerRepository, CustomerDynamoDBRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentDynamoDBRepository>();
            services.AddScoped<IDerivedOfTRepository, DerivedOfTDynamoDBRepository>();
            services.AddScoped<IDerivedTypeRepository, DerivedTypeDynamoDBRepository>();
            services.AddScoped<IDerivedTypeAggregateRepository, DerivedTypeAggregateDynamoDBRepository>();
            // IntentIgnore(Match = "services.AddScoped<IEntityOfTRepository")
            services.AddScoped(typeof(IEntityOfTRepository<>), typeof(EntityOfTDynamoDBRepository<>));
            services.AddScoped<IIdTestingRepository, IdTestingDynamoDBRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceDynamoDBRepository>();
            services.AddScoped<INonStringPartitionKeyRepository, NonStringPartitionKeyDynamoDBRepository>();
            services.AddScoped<IOrderRepository, OrderDynamoDBRepository>();
            services.AddScoped<IPackageContainerRepository, PackageContainerDynamoDBRepository>();
            services.AddScoped<IProductRepository, ProductDynamoDBRepository>();
            services.AddScoped<IRegionRepository, RegionDynamoDBRepository>();
            services.AddScoped<IUniversityRepository, UniversityDynamoDBRepository>();
            services.AddScoped<IWithGuidIdRepository, WithGuidIdDynamoDBRepository>();
            services.AddScoped<IWithoutPartitionKeyRepository, WithoutPartitionKeyDynamoDBRepository>();
            services.AddScoped<IFolderContainerRepository, FolderContainerDynamoDBRepository>();
            services.AddScoped<IOnDemandRepository, OnDemandDynamoDBRepository>();
            services.AddScoped<IProvisionedRepository, ProvisionedDynamoDBRepository>();
            services.AddScoped<DynamoDBUnitOfWork>();
            services.AddScoped<IDynamoDBUnitOfWork>(provider => provider.GetRequiredService<DynamoDBUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.ConfigureAws(configuration);
            return services;
        }
    }
}