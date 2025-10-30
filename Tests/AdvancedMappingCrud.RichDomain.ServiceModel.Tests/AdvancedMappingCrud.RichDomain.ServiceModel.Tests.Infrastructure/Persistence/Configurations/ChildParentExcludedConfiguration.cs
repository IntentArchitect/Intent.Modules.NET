using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Persistence.Configurations
{
    public class ChildParentExcludedConfiguration : IEntityTypeConfiguration<ChildParentExcluded>
    {
        public void Configure(EntityTypeBuilder<ChildParentExcluded> builder)
        {
            builder.ToTable("ChildParentExcludeds");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChildName)
                .IsRequired();

            builder.Property(x => x.ParentAge)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}