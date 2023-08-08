using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RichDomain.Infrastructure.Persistence.Configurations
{
    public class DerivedClassConfiguration : IEntityTypeConfiguration<DerivedClass>
    {
        public void Configure(EntityTypeBuilder<DerivedClass> builder)
        {
            builder.HasBaseType<BaseClass>();

            builder.Property(x => x.DerivedAttribute)
                .IsRequired();
        }
    }
}