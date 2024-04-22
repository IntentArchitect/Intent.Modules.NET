using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common.Interfaces;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations.PrimaryKeyTypes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<SpResult> SpResults { get; set; }
        public DbSet<EntityRecord> EntityRecords { get; set; }

        public DbSet<AggregateRoot1> AggregateRoot1s { get; set; }
        public DbSet<AggregateRoot2Composition> AggregateRoot2Compositions { get; set; }
        public DbSet<AggregateRoot3AggCollection> AggregateRoot3AggCollections { get; set; }
        public DbSet<AggregateRoot3Collection> AggregateRoot3Collections { get; set; }
        public DbSet<AggregateRoot3Nullable> AggregateRoot3Nullables { get; set; }
        public DbSet<AggregateRoot3Single> AggregateRoot3Singles { get; set; }
        public DbSet<AggregateRoot4AggNullable> AggregateRoot4AggNullables { get; set; }
        public DbSet<AggregateRoot4Collection> AggregateRoot4Collections { get; set; }
        public DbSet<AggregateRoot4Nullable> AggregateRoot4Nullables { get; set; }
        public DbSet<AggregateRoot4Single> AggregateRoot4Singles { get; set; }
        public DbSet<AggregateRoot5> AggregateRoot5s { get; set; }
        public DbSet<MockEntity> MockEntities { get; set; }
        public DbSet<NewClassByte> NewClassBytes { get; set; }
        public DbSet<NewClassDecimal> NewClassDecimals { get; set; }
        public DbSet<NewClassDouble> NewClassDoubles { get; set; }
        public DbSet<NewClassFloat> NewClassFloats { get; set; }
        public DbSet<NewClassGuid> NewClassGuids { get; set; }
        public DbSet<NewClassInt> NewClassInts { get; set; }
        public DbSet<NewClassLong> NewClassLongs { get; set; }
        public DbSet<NewClassShort> NewClassShorts { get; set; }

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
            modelBuilder.Entity<SpResult>().HasNoKey().ToView(null);
            modelBuilder.Entity<EntityRecord>().HasNoKey().ToView(null);
            modelBuilder.ApplyConfiguration(new AggregateRoot1Configuration());
            modelBuilder.ApplyConfiguration(new AggregateRoot2CompositionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3AggCollectionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3CollectionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3NullableConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3SingleConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4AggNullableConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4CollectionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4NullableConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4SingleConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot5Configuration());
            modelBuilder.ApplyConfiguration(new MockEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassByteConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassDecimalConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassDoubleConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassFloatConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassGuidConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassIntConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassLongConfiguration());
            modelBuilder.ApplyConfiguration(new NewClassShortConfiguration());
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
            new Car() { CarId = 3, Make = "Labourghini", Model = "Countach" });
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

        public async Task<T?> ExecuteScalarAsync<T>(string rawSql, params DbParameter[]? parameters)
        {
            var connection = Database.GetDbConnection();
            await using var command = connection.CreateCommand();

            command.CommandText = rawSql;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            await connection.OpenAsync();
            return (T?)await command.ExecuteScalarAsync();
        }
    }
}