using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        public DbSet<DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        public DbSet<DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        public DbSet<FkAssociatedClass> FkAssociatedClasses { get; set; }
        public DbSet<FkBaseClass> FkBaseClasses { get; set; }
        public DbSet<FkDerivedClass> FkDerivedClasses { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);

            modelBuilder.ApplyConfiguration(new ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new FkDerivedClassConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Customize Default Schema
            // modelBuilder.HasDefaultSchema("EntityFrameworkCore.TPC.TestApplication");

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