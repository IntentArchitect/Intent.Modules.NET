using CqrsSpike.Subscriber.Domain.Common.Interfaces;
using CqrsSpike.Subscriber.Domain.Entities;
using CqrsSpike.Subscriber.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CqrsSpike.Subscriber.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<OrderRecord> OrderRecords { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

        public bool HasDbTransaction() => Database.CurrentTransaction != null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new OrderRecordConfiguration());
            modelBuilder.ApplyConfiguration(new ShipmentConfiguration());
        }

        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Seed data
            // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
            /* E.g.
            modelBuilder.Entity<Car>().HasData(
                new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
                new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
                new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }
    }
}