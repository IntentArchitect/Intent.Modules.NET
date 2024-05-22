using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.MappingTests
{
    public class NestingParentConfiguration : IEntityTypeConfiguration<NestingParent>
    {
        public void Configure(EntityTypeBuilder<NestingParent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.NestingChildren, ConfigureNestingChildren);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureNestingChildren(OwnedNavigationBuilder<NestingParent, NestingChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.NestingParentId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.NestingParentId)
                .IsRequired();

            builder.OwnsOne(x => x.NestingChildChild, ConfigureNestingChildChild)
                .Navigation(x => x.NestingChildChild).IsRequired();
        }

        public void ConfigureNestingChildChild(OwnedNavigationBuilder<NestingChild, NestingChildChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}