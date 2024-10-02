using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using SqlServerImporterTests.Application.Common.Interfaces;
using SqlServerImporterTests.Domain.Common;
using SqlServerImporterTests.Domain.Common.Interfaces;
using SqlServerImporterTests.Domain.Contracts.Dbo;
using SqlServerImporterTests.Domain.Entities.Dbo;
using SqlServerImporterTests.Domain.Entities.Schema2;
using SqlServerImporterTests.Domain.Entities.Views;
using SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo;
using SqlServerImporterTests.Infrastructure.Persistence.Configurations.Schema2;
using SqlServerImporterTests.Infrastructure.Persistence.Configurations.Views;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<GetCustomerOrdersResponse> GetCustomerOrdersResponses { get; set; }

        public DbSet<GetOrderItemDetailsResponse> GetOrderItemDetailsResponses { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Domain.Entities.Dbo.Customer> DboCustomers { get; set; }
        public DbSet<LegacyTable> LegacyTables { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<VwOrder> VwOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Banks> Banks { get; set; }
        public DbSet<Domain.Entities.Schema2.Customer> Schema2Customers { get; set; }

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
            modelBuilder.Entity<GetCustomerOrdersResponse>().HasNoKey().ToView(null);
            modelBuilder.Entity<GetOrderItemDetailsResponse>().HasNoKey().ToView(null);
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetRoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserLoginConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new ChildConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new LegacyTableConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new ParentConfiguration());
            modelBuilder.ApplyConfiguration(new PriceConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new BankConfiguration());
            modelBuilder.ApplyConfiguration(new BanksConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Schema2.CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CustomersConfiguration());
            modelBuilder.ApplyConfiguration(new VwOrderConfiguration());
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
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
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