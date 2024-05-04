using EntityFrameworkCore.MultiDbContext.DbContextInterface.Application.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Persistence
{
    public class ConnStrDbContext : DbContext, IConnStrDbContext, IUnitOfWork
    {
        public ConnStrDbContext(DbContextOptions<ConnStrDbContext> options) : base(options)
        {
        }

        public DbSet<ConnstrEntity> ConnstrEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new ConnstrEntityConfiguration());
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
                new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }
    }
}