using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Finbuckle.SharedDatabase.TestApplication.Domain.Common.Interfaces;
using Finbuckle.SharedDatabase.TestApplication.Domain.Entities;
using Finbuckle.SharedDatabase.TestApplication.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork, IMultiTenantDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantInfo tenantInfo) : base(options)
        {
            TenantInfo = tenantInfo;
        }

        public DbSet<User> Users { get; set; }
        public ITenantInfo TenantInfo { get; private set; }
        public TenantMismatchMode TenantMismatchMode { get; set; } = TenantMismatchMode.Throw;
        public TenantNotSetMode TenantNotSetMode { get; set; } = TenantNotSetMode.Throw;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.EnforceMultiTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.EnforceMultiTenant();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

    }
}