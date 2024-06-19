using System;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Domain.Common.Interfaces;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Domain.Entities;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Infrastructure.Persistence.Configurations;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Infrastructure.Persistence.Configurations.Converters;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("pgsql");

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
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

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties(typeof(DateTimeOffset)).HaveConversion(typeof(UtcDateTimeOffsetConverter));
        }
    }
}