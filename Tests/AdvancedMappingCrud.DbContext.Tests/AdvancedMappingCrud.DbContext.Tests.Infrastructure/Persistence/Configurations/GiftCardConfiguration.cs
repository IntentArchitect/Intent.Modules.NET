using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Infrastructure.Persistence.Configurations
{
    public class GiftCardConfiguration : IEntityTypeConfiguration<GiftCard>
    {
        public void Configure(EntityTypeBuilder<GiftCard> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.CustomerId);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}