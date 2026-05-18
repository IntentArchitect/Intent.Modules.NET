using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Persistence.Configurations
{
    public class ContainerConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContainerNumber)
                .IsRequired();

            builder.Property(x => x.SealNumber)
                .IsRequired();

            builder.OwnsMany(x => x.Vessels, ConfigureVessels);
        }

        public static void ConfigureVessels(OwnedNavigationBuilder<Container, Vessel> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ContainerId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContainerId)
                .IsRequired();

            builder.Property(x => x.IMOCode)
                .IsRequired();
        }
    }
}