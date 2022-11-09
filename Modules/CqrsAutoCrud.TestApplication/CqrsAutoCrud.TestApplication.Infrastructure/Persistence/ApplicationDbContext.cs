using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Common.Interfaces;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<AggregateRoot> AggregateRoots { get; set; }
        public DbSet<AggregateRootLong> AggregateRootLongs { get; set; }
        public DbSet<AggregateSingleC> AggregateSingleCs { get; set; }
        public DbSet<CompositeSingleA> CompositeSingleAs { get; set; }
        public DbSet<CompositeSingleAA> CompositeSingleAAs { get; set; }
        public DbSet<CompositeSingleBB> CompositeSingleBBs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new AggregateRootConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRootLongConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateSingleCConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleBBConfiguration());
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