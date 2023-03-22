using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations
{
    public class VariantTypesClassConfiguration : IEntityTypeConfiguration<VariantTypesClass>
    {
        public void Configure(EntityTypeBuilder<VariantTypesClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StrCollection)
                .IsRequired();

            builder.Property(x => x.IntCollection)
                .IsRequired();

            builder.Property(x => x.StrNullCollection);

            builder.Property(x => x.IntNullCollection);

            builder.Property(x => x.NullStr);

            builder.Property(x => x.NullInt);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}