using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using EfCoreRepositoryTestSuite.IntentGenerated.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Core
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AggregateRoot1> AggregateRoot1s { get; set; }
        public DbSet<AggregateRoot2Collection> AggregateRoot2Collections { get; set; }
        public DbSet<AggregateRoot2Composition> AggregateRoot2Compositions { get; set; }
        public DbSet<AggregateRoot2Nullable> AggregateRoot2Nullables { get; set; }
        public DbSet<AggregateRoot2Single> AggregateRoot2Singles { get; set; }
        public DbSet<AggregateRoot3AggCollection> AggregateRoot3AggCollections { get; set; }
        public DbSet<AggregateRoot3Collection> AggregateRoot3Collections { get; set; }
        public DbSet<AggregateRoot3Nullable> AggregateRoot3Nullables { get; set; }
        public DbSet<AggregateRoot3Single> AggregateRoot3Singles { get; set; }
        public DbSet<AggregateRoot4AggNullable> AggregateRoot4AggNullables { get; set; }
        public DbSet<AggregateRoot4Collection> AggregateRoot4Collections { get; set; }
        public DbSet<AggregateRoot4Nullable> AggregateRoot4Nullables { get; set; }
        public DbSet<AggregateRoot4Single> AggregateRoot4Singles { get; set; }
        public DbSet<AggregateRoot5> AggregateRoot5s { get; set; }
        public DbSet<AggregateRoot5EntityWithRepo> AggregateRoot5EntityWithRepos { get; set; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);

            modelBuilder.ApplyConfiguration(new AggregateRoot1Configuration());
            modelBuilder.ApplyConfiguration(new AggregateRoot2CollectionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot2CompositionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot2NullableConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot2SingleConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3AggCollectionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3CollectionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3NullableConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot3SingleConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4AggNullableConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4CollectionConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4NullableConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot4SingleConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRoot5Configuration());
            modelBuilder.ApplyConfiguration(new AggregateRoot5EntityWithRepoConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Customize Default Schema
            // modelBuilder.HasDefaultSchema("EntityFrameworkCore.Repositories.TestApplication");

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