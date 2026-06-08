using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Infrastructure.Persistence.Configurations
{
    public class SpecializedProductConfiguration : IEntityTypeConfiguration<SpecializedProduct>
    {
        public void Configure(EntityTypeBuilder<SpecializedProduct> builder)
        {
            builder.HasBaseType<Product>();

            builder.Property(x => x.Code)
                .IsRequired();
        }
    }
}