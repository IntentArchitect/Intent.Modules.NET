using AzureFunctions.NET6.Domain.Common.Interfaces;
using AzureFunctions.NET6.Domain.Entities;
using AzureFunctions.NET6.Infrastructure.Persistence.Configurations;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using AzureFunctions.NET8.Domain.Entities;
using AzureFunctions.NET8.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace AzureFunctions.NET8.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<SampleDomain> SampleDomains { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new SampleDomainConfiguration());
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