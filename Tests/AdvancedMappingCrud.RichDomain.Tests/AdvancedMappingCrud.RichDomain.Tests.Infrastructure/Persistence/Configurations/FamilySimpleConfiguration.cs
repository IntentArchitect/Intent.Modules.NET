using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Infrastructure.Persistence.Configurations
{
    public class FamilySimpleConfiguration : IEntityTypeConfiguration<FamilySimple>
    {
        public void Configure(EntityTypeBuilder<FamilySimple> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChildName)
                .IsRequired();

            builder.Property(x => x.ParentId)
                .IsRequired();

            builder.Property(x => x.ParentName)
                .IsRequired();

            builder.Property(x => x.GrandparentId)
                .IsRequired();

            builder.Property(x => x.GrandparentName)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}