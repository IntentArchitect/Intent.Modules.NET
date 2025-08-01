using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.AnemicChild;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.Indexing;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.MappingTests;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.OData.SimpleKey;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ServiceToServiceInvocations;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Caching;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Configuration;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.AnemicChild;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.Indexing;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.MappingTests;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.OData.SimpleKey;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.ServiceToServiceInvocations;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure
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
            services.AddDistributedMemoryCache();
            services.AddSingleton<IDistributedCacheWithUnitOfWork, DistributedCacheWithUnitOfWork>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IServiceStoredProcRepository, ServiceStoredProcRepository>();
            services.AddTransient<IBasicRepository, BasicRepository>();
            services.AddTransient<ICompanyContactRepository, CompanyContactRepository>();
            services.AddTransient<ICompanyContactSecondRepository, CompanyContactSecondRepository>();
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<IContactSecondRepository, ContactSecondRepository>();
            services.AddTransient<IContractRepository, ContractRepository>();
            services.AddTransient<ICorporateFuneralCoverQuoteRepository, CorporateFuneralCoverQuoteRepository>();
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IEntityListEnumRepository, EntityListEnumRepository>();
            services.AddTransient<IFileUploadRepository, FileUploadRepository>();
            services.AddTransient<IFuneralCoverQuoteRepository, FuneralCoverQuoteRepository>();
            services.AddTransient<IMultiKeyParentRepository, MultiKeyParentRepository>();
            services.AddTransient<IOptionalRepository, OptionalRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IPagingTSRepository, PagingTSRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IQuoteRepository, QuoteRepository>();
            services.AddTransient<Domain.Repositories.IUserRepository, Repositories.UserRepository>();
            services.AddTransient<IWarehouseRepository, WarehouseRepository>();
            services.AddTransient<IParentWithAnemicChildRepository, ParentWithAnemicChildRepository>();
            services.AddTransient<IFarmerRepository, FarmerRepository>();
            services.AddTransient<IClassicDomainServiceTestRepository, ClassicDomainServiceTestRepository>();
            services.AddTransient<IDomainServiceTestRepository, DomainServiceTestRepository>();
            services.AddTransient<IBaseEntityARepository, BaseEntityARepository>();
            services.AddTransient<IBaseEntityBRepository, BaseEntityBRepository>();
            services.AddTransient<IConcreteEntityARepository, ConcreteEntityARepository>();
            services.AddTransient<IConcreteEntityBRepository, ConcreteEntityBRepository>();
            services.AddTransient<IFilteredIndexRepository, FilteredIndexRepository>();
            services.AddTransient<INestingParentRepository, NestingParentRepository>();
            services.AddTransient<IODataCustomerRepository, ODataCustomerRepository>();
            services.AddTransient<IODataProductRepository, ODataProductRepository>();
            services.AddTransient<Domain.Repositories.OperationMapping.IUserRepository, Repositories.OperationMapping.UserRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            services.AddHttpClients(configuration);
            return services;
        }
    }
}