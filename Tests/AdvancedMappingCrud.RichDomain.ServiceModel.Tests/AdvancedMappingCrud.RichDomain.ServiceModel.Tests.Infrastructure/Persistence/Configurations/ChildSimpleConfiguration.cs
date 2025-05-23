using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Persistence.Configurations
{
    public class ChildSimpleConfiguration : IEntityTypeConfiguration<ChildSimple>
    {
        public void Configure(EntityTypeBuilder<ChildSimple> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChildName)
                .IsRequired();

            builder.Property(x => x.ParentName)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}