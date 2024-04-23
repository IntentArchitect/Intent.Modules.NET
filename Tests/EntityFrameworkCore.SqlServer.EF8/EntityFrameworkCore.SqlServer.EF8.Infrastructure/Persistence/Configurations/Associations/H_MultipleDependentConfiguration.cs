using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Associations
{
    public class H_MultipleDependentConfiguration : IEntityTypeConfiguration<H_MultipleDependent>
    {
        public void Configure(EntityTypeBuilder<H_MultipleDependent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleDepAttr)
                .IsRequired();

            builder.Property(x => x.H_OptionalAggregateNavId);
        }
    }
}