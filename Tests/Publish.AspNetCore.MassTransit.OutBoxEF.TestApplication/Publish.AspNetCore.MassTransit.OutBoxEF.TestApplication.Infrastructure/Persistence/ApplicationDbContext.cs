using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Common.Interfaces;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities.Mapping;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Persistence.Configurations;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Persistence.Configurations.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IEventBus _eventBus;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEventBus eventBus) : base(options)
        {
            _eventBus = eventBus;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ClassWithVO> ClassWithVOs { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await _eventBus.FlushAllAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _eventBus.FlushAllAsync().GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithVOConfiguration());
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
    }
}