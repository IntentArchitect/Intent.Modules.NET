using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.CRUD
{
    public class CompositeSingleAAConfiguration : IEntityTypeConfiguration<CompositeSingleAA>
    {
        public void Configure(EntityTypeBuilder<CompositeSingleAA> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }
    }
}