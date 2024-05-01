using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Common.Interfaces;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Entities;
using Entities.PrivateSetters.EF.CosmosDb.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
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

        /// <summary>
        /// Calling EnsureCreatedAsync is necessary to create the required containers and insert the seed data if present in the model.
        /// However EnsureCreatedAsync should only be called during deployment, not normal operation, as it may cause performance issues.
        /// </summary>
        public async Task EnsureDbCreatedAsync()
        {
            await Database.EnsureCreatedAsync();
        }
    }
}