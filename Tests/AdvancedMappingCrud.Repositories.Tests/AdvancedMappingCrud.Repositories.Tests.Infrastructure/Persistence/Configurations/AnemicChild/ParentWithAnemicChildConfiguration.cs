using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.AnemicChild;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.AnemicChild
{
    public class ParentWithAnemicChildConfiguration : IEntityTypeConfiguration<ParentWithAnemicChild>
    {
        public void Configure(EntityTypeBuilder<ParentWithAnemicChild> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Surname)
                .IsRequired();

            builder.OwnsMany(x => x.AnemicChildren, ConfigureAnemicChildren);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAnemicChildren(OwnedNavigationBuilder<ParentWithAnemicChild, Domain.Entities.AnemicChild.AnemicChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ParentWithAnemicChildId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ParentWithAnemicChildId)
                .IsRequired();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();
        }
    }
}