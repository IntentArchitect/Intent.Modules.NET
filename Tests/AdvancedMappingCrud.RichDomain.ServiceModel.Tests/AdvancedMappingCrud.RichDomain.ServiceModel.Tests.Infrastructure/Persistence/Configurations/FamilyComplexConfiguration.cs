using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Persistence.Configurations
{
    public class FamilyComplexConfiguration : IEntityTypeConfiguration<FamilyComplex>
    {
        public void Configure(EntityTypeBuilder<FamilyComplex> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChildName)
                .IsRequired();

            builder.Property(x => x.ParentId)
                .IsRequired();

            builder.Property(x => x.ParentName)
                .IsRequired();

            builder.Property(x => x.GrandParentId)
                .IsRequired();

            builder.Property(x => x.GreatGrandParentId)
                .IsRequired();

            builder.Property(x => x.GreatGrandParentName)
                .IsRequired();

            builder.Property(x => x.AuntId)
                .IsRequired();

            builder.Property(x => x.AuntName)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}