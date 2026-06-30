using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace ObjectMappingTest.Infrastructure.Persistence.Configurations
{
    public class DigitalProductConfiguration : IEntityTypeConfiguration<DigitalProduct>
    {
        public void Configure(EntityTypeBuilder<DigitalProduct> builder)
        {
            builder.HasBaseType<Product>();

            builder.Property(x => x.DownloadUrl)
                .IsRequired();
        }
    }
}