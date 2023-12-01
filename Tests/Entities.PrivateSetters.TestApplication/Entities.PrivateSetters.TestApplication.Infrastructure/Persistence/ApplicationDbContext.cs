using Entities.PrivateSetters.TestApplication.Domain.Common.Interfaces;
using Entities.PrivateSetters.TestApplication.Domain.Entities;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping;
using Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations;
using Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Aggregational;
using Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Compositional;
using Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Mapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ManyToManyDest> ManyToManyDests { get; set; }
        public DbSet<ManyToManySource> ManyToManySources { get; set; }
        public DbSet<ManyToOneDest> ManyToOneDests { get; set; }
        public DbSet<ManyToOneSource> ManyToOneSources { get; set; }
        public DbSet<OptionalToManyDest> OptionalToManyDests { get; set; }
        public DbSet<OptionalToManySource> OptionalToManySources { get; set; }
        public DbSet<OptionalToOneDest> OptionalToOneDests { get; set; }
        public DbSet<OptionalToOneSource> OptionalToOneSources { get; set; }
        public DbSet<OneToManySource> OneToManySources { get; set; }
        public DbSet<OneToOneSource> OneToOneSources { get; set; }
        public DbSet<OneToOptionalSource> OneToOptionalSources { get; set; }
        public DbSet<MappingRoot> MappingRoots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new ManyToManyDestConfiguration());
            modelBuilder.ApplyConfiguration(new ManyToManySourceConfiguration());
            modelBuilder.ApplyConfiguration(new ManyToOneDestConfiguration());
            modelBuilder.ApplyConfiguration(new ManyToOneSourceConfiguration());
            modelBuilder.ApplyConfiguration(new OptionalToManyDestConfiguration());
            modelBuilder.ApplyConfiguration(new OptionalToManySourceConfiguration());
            modelBuilder.ApplyConfiguration(new OptionalToOneDestConfiguration());
            modelBuilder.ApplyConfiguration(new OptionalToOneSourceConfiguration());
            modelBuilder.ApplyConfiguration(new OneToManySourceConfiguration());
            modelBuilder.ApplyConfiguration(new OneToOneSourceConfiguration());
            modelBuilder.ApplyConfiguration(new OneToOptionalSourceConfiguration());
            modelBuilder.ApplyConfiguration(new MappingRootConfiguration());
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