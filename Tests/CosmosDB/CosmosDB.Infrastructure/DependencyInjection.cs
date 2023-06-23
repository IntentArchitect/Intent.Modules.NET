using System;
using System.Reflection;
using AutoMapper;
using CosmosDB.Application;
using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain.Common.Interfaces;
using CosmosDB.Domain.Repositories;
using CosmosDB.Domain.Repositories.Folder;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using CosmosDB.Infrastructure.Persistence.Documents.Folder;
using CosmosDB.Infrastructure.Repositories;
using CosmosDB.Infrastructure.Repositories.Folder;
using CosmosDB.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CosmosDB.Infrastructure
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
                    .Configure<ClassContainerDocument>(c => c
                        .WithContainer("Class")
                        .WithPartitionKey("/classPartitionKey"))
                    .Configure<ClientDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<FolderContainerDocument>(c => c
                        .WithContainer("Folder")
                        .WithPartitionKey("/folderPartitionKey"))
                    .Configure<IdTestingDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<InvoiceDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<PackageContainerDocument>(c => c
                        .WithContainer("PackageContainer")
                        .WithPartitionKey("/packagePartitionKey"))
                    .Configure<WithoutPartitionKeyDocument>(c => c
                        .WithContainer("WithoutPartitionKey"));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(Application.DependencyInjection).Assembly);
            services.AddScoped<IClassContainerRepository, ClassContainerCosmosDBRepository>();
            services.AddScoped<IClientRepository, ClientCosmosDBRepository>();
            services.AddScoped<IIdTestingRepository, IdTestingCosmosDBRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceCosmosDBRepository>();
            services.AddScoped<IPackageContainerRepository, PackageContainerCosmosDBRepository>();
            services.AddScoped<IWithoutPartitionKeyRepository, WithoutPartitionKeyCosmosDBRepository>();
            services.AddScoped<IFolderContainerRepository, FolderContainerCosmosDBRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            return services;
        }
    }
}