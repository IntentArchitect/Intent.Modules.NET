using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Persistence.Configurations
{
    public class FamilyComplexSkippedConfiguration : IEntityTypeConfiguration<FamilyComplexSkipped>
    {
        public void Configure(EntityTypeBuilder<FamilyComplexSkipped> builder)
        {
            builder.ToTable("FamilyComplexSkippeds");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChildName)
                .IsRequired();

            builder.Property(x => x.ParentId)
                .IsRequired();

            builder.Property(x => x.GrandparentId)
                .IsRequired();

            builder.Property(x => x.GreatGrandparentId)
                .IsRequired();

            builder.Property(x => x.GreatGrandparentName)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}