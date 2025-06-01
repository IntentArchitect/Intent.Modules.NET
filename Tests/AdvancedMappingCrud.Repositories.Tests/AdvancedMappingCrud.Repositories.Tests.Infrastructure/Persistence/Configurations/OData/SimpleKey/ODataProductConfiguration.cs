using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.OData.SimpleKey
{
    public class ODataProductConfiguration : IEntityTypeConfiguration<ODataProduct>
    {
        public void Configure(EntityTypeBuilder<ODataProduct> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.Price)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}