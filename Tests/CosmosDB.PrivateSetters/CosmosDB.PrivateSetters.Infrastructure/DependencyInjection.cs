using System;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using CosmosDB.PrivateSetters.Domain.Common.Interfaces;
using CosmosDB.PrivateSetters.Domain.Repositories;
using CosmosDB.PrivateSetters.Domain.Repositories.Folder;
using CosmosDB.PrivateSetters.Infrastructure.Persistence;
using CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents;
using CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents.Folder;
using CosmosDB.PrivateSetters.Infrastructure.Repositories;
using CosmosDB.PrivateSetters.Infrastructure.Repositories.Folder;
using CosmosDB.PrivateSetters.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository(options =>
            {
                var defaultContainerId = configuration.GetValue<string>("RepositoryOptions:ContainerId");

                if (string.IsNullOrWhiteSpace(defaultContainerId))
                {
                    throw new Exception("\"RepositoryOptions:ContainerId\" configuration not specified");
                }

                options.ContainerPerItemType = true;

                options.ContainerBuilder
                    .Configure<BaseTypeDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<CategoryDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<ClassContainerDocument>(c => c
                        .WithContainer("Class")
                        .WithPartitionKey("/classPartitionKey"))
                    .Configure<ClientDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<CustomerDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<DerivedOfTDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<DerivedTypeDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<DerivedTypeAggregateDocument>(c => c
                        .WithContainer(defaultContainerId))
                    //.Configure<EntityOfTDocument>(c => c
                    //    .WithContainer(defaultContainerId))
                    .Configure<FolderContainerDocument>(c => c
                        .WithContainer("Folder")
                        .WithPartitionKey("/folderPartitionKey"))
                    .Configure<IdTestingDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<InvoiceDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<NonStringPartitionKeyDocument>(c => c
                        .WithContainer("NonStringPartitionKey")
                        .WithPartitionKey("/partInt"))
                    .Configure<OrderDocument>(c => c
                        .WithContainer("Order")
                        .WithPartitionKey("/warehouseId"))
                    .Configure<PackageContainerDocument>(c => c
                        .WithContainer("PackageContainer")
                        .WithPartitionKey("/packagePartitionKey"))
                    .Configure<ProductDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<RegionDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<WithGuidIdDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<WithoutPartitionKeyDocument>(c => c
                        .WithContainer("WithoutPartitionKey"));
            });
            services.AddScoped<IBaseTypeRepository, BaseTypeCosmosDBRepository>();
            services.AddScoped<ICategoryRepository, CategoryCosmosDBRepository>();
            services.AddScoped<IClassContainerRepository, ClassContainerCosmosDBRepository>();
            services.AddScoped<IClientRepository, ClientCosmosDBRepository>();
            services.AddScoped<ICustomerRepository, CustomerCosmosDBRepository>();
            services.AddScoped<IDerivedOfTRepository, DerivedOfTCosmosDBRepository>();
            services.AddScoped<IDerivedTypeRepository, DerivedTypeCosmosDBRepository>();
            services.AddScoped<IDerivedTypeAggregateRepository, DerivedTypeAggregateCosmosDBRepository>();
            //services.AddScoped<IEntityOfTRepository, EntityOfTCosmosDBRepository>();
            services.AddScoped<IIdTestingRepository, IdTestingCosmosDBRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceCosmosDBRepository>();
            services.AddScoped<INonStringPartitionKeyRepository, NonStringPartitionKeyCosmosDBRepository>();
            services.AddScoped<IOrderRepository, OrderCosmosDBRepository>();
            services.AddScoped<IPackageContainerRepository, PackageContainerCosmosDBRepository>();
            services.AddScoped<IProductRepository, ProductCosmosDBRepository>();
            services.AddScoped<IRegionRepository, RegionCosmosDBRepository>();
            services.AddScoped<IWithGuidIdRepository, WithGuidIdCosmosDBRepository>();
            services.AddScoped<IWithoutPartitionKeyRepository, WithoutPartitionKeyCosmosDBRepository>();
            services.AddScoped<IFolderContainerRepository, FolderContainerCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}