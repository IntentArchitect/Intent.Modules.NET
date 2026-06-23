using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using N_ServiceBus.Persistence.Sql.Subscribe.Application.Common.Eventing;
using N_ServiceBus.Persistence.Sql.Subscribe.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace N_ServiceBus.Persistence.Sql.Subscribe.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IMessageBus _messageBus;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMessageBus messageBus) : base(options)
        {
            _messageBus = messageBus;
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await _messageBus.FlushAllAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _messageBus.FlushAllAsync().GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public bool HasDbTransaction() => Database.CurrentTransaction != null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Seed data
            // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
            /* E.g.
            modelBuilder.Entity<Car>().HasData(
                new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
                new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
                new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }
    }
}