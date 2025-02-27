using AdvancedMappingCrud.Repositories.Tests.Domain.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.DomainInvoke
{
    public class FarmerConfiguration : IEntityTypeConfiguration<Farmer>
    {
        public void Configure(EntityTypeBuilder<Farmer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Surname)
                .IsRequired();

            builder.OwnsMany(x => x.Plots, ConfigurePlots);

            builder.OwnsMany(x => x.Machines, ConfigureMachines);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigurePlots(OwnedNavigationBuilder<Farmer, Plot> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();
        }

        public static void ConfigureMachines(OwnedNavigationBuilder<Farmer, Machines> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.FarmerId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.FarmerId)
                .IsRequired();
        }
    }
}