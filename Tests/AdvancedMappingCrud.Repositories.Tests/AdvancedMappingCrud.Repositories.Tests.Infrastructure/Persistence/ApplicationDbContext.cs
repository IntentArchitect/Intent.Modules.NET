using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.AnemicChild;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Indexing;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.AnemicChild;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.EdgeCompositeHandling;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.Indexing;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.MappingTests;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.NullableNested;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.OData.SimpleKey;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.OperationMapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<Basic> Basics { get; set; }
        public DbSet<CompanyContact> CompanyContacts { get; set; }
        public DbSet<CompanyContactSecond> CompanyContactSeconds { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactSecond> ContactSeconds { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<CorporateFuneralCoverQuote> CorporateFuneralCoverQuotes { get; set; }
        public DbSet<Country> Countries { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<EntityListEnum> EntityListEnums { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<FuneralCoverQuote> FuneralCoverQuotes { get; set; }
        public DbSet<MultiKeyParent> MultiKeyParents { get; set; }
        public DbSet<Optional> Optionals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PagingTS> PagingTs { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Domain.Entities.User> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ParentWithAnemicChild> ParentWithAnemicChildren { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<ClassicDomainServiceTest> ClassicDomainServiceTests { get; set; }
        public DbSet<DomainServiceTest> DomainServiceTests { get; set; }
        public DbSet<External> Externals { get; set; }
        public DbSet<Level1> Level1s { get; set; }
        public DbSet<Level2> Level2s { get; set; }
        public DbSet<Root> Roots { get; set; }
        public DbSet<BaseEntityA> BaseEntityAs { get; set; }
        public DbSet<BaseEntityB> BaseEntityBs { get; set; }
        public DbSet<ConcreteEntityA> ConcreteEntityAs { get; set; }
        public DbSet<ConcreteEntityB> ConcreteEntityBs { get; set; }
        public DbSet<FilteredIndex> FilteredIndices { get; set; }
        public DbSet<NestingParent> NestingParents { get; set; }
        public DbSet<One> Ones { get; set; }
        public DbSet<ODataCustomer> ODataCustomers { get; set; }
        public DbSet<ODataProduct> ODataProducts { get; set; }
        public DbSet<Domain.Entities.OperationMapping.User> OperationMappingUsers { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DispatchEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DispatchEventsAsync().GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new BasicConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyContactConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyContactSecondConfiguration());
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new ContactSecondConfiguration());
            modelBuilder.ApplyConfiguration(new ContractConfiguration());
            modelBuilder.ApplyConfiguration(new CorporateFuneralCoverQuoteConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new EntityListEnumConfiguration());
            modelBuilder.ApplyConfiguration(new FileUploadConfiguration());
            modelBuilder.ApplyConfiguration(new FuneralCoverQuoteConfiguration());
            modelBuilder.ApplyConfiguration(new MultiKeyParentConfiguration());
            modelBuilder.ApplyConfiguration(new OptionalConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new PagingTSConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
            modelBuilder.ApplyConfiguration(new ParentWithAnemicChildConfiguration());
            modelBuilder.ApplyConfiguration(new FarmerConfiguration());
            modelBuilder.ApplyConfiguration(new ClassicDomainServiceTestConfiguration());
            modelBuilder.ApplyConfiguration(new DomainServiceTestConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalConfiguration());
            modelBuilder.ApplyConfiguration(new Level1Configuration());
            modelBuilder.ApplyConfiguration(new Level2Configuration());
            modelBuilder.ApplyConfiguration(new RootConfiguration());
            modelBuilder.ApplyConfiguration(new BaseEntityAConfiguration());
            modelBuilder.ApplyConfiguration(new BaseEntityBConfiguration());
            modelBuilder.ApplyConfiguration(new ConcreteEntityAConfiguration());
            modelBuilder.ApplyConfiguration(new ConcreteEntityBConfiguration());
            modelBuilder.ApplyConfiguration(new FilteredIndexConfiguration());
            modelBuilder.ApplyConfiguration(new NestingParentConfiguration());
            modelBuilder.ApplyConfiguration(new OneConfiguration());
            modelBuilder.ApplyConfiguration(new ODataCustomerConfiguration());
            modelBuilder.ApplyConfiguration(new ODataProductConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.OperationMapping.UserConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Seed data
            // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
            /* Eg.
            
            modelBuilder.Entity<Car>().HasData(
            new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
            new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
            new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }

        private async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .SelectMany(x => x.Entity.DomainEvents)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity is null)
                {
                    break;
                }

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
            }
        }
    }
}