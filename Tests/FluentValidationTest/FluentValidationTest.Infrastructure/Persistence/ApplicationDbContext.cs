using FluentValidationTest.Domain.Common.Interfaces;
using FluentValidationTest.Domain.Entities.ValidationScenarios.ConstructorOperationConstraints;
using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Domain.Entities.ValidationScenarios.Nullability;
using FluentValidationTest.Domain.Entities.ValidationScenarios.NumericConstraints;
using FluentValidationTest.Domain.Entities.ValidationScenarios.PatternConstraints;
using FluentValidationTest.Domain.Entities.ValidationScenarios.TextConstraints;
using FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.ConstructorOperationConstraints;
using FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.Nullability;
using FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.NumericConstraints;
using FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.PatternConstraints;
using FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.TextConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ConstructedConstrainedEntity> ConstructedConstrainedEntities { get; set; }
        public DbSet<UniqueAccountEntity> UniqueAccountEntities { get; set; }
        public DbSet<UniquePersonEntity> UniquePersonEntities { get; set; }
        public DbSet<NullabilityConstrainedEntity> NullabilityConstrainedEntities { get; set; }
        public DbSet<NumericConstrainedEntity> NumericConstrainedEntities { get; set; }
        public DbSet<PatternConstrainedEntity> PatternConstrainedEntities { get; set; }
        public DbSet<TextConstrainedEntity> TextConstrainedEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new ConstructedConstrainedEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UniqueAccountEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UniquePersonEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NullabilityConstrainedEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NumericConstrainedEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PatternConstrainedEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TextConstrainedEntityConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
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