using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IOptions<DbContextConfiguration> _dbContextConfig;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IOptions<DbContextConfiguration> dbContextConfig) : base(options)
        {
            _dbContextConfig = dbContextConfig;
        }

        public DbSet<Associated> Associateds { get; set; }
        public DbSet<BaseAssociated> BaseAssociateds { get; set; }
        public DbSet<Derived> Deriveds { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);

            modelBuilder.ApplyConfiguration(new AssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new BaseAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedConfiguration());
            if (!string.IsNullOrWhiteSpace(_dbContextConfig.Value?.DefaultContainerName))
            {
                modelBuilder.HasDefaultContainer(_dbContextConfig.Value?.DefaultContainerName);
            }
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
        /// If configured to do so, a check is performed to see
        /// whether the database exist and if not will create it
        /// based on this container configuration.
        /// </summary>
        public void EnsureDbCreated()
        {
            if (_dbContextConfig.Value.EnsureDbCreated == true)
            {
                Database.EnsureCreated();
            }
        }
    }
}