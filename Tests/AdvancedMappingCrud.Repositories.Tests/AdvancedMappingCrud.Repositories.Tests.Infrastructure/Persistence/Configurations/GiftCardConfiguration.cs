using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class GiftCardConfiguration : IEntityTypeConfiguration<GiftCard>
    {
        public void Configure(EntityTypeBuilder<GiftCard> builder)
        {
            builder.HasKey(x => x.CardCode);

            builder.Property(x => x.CardCode)
                .ValueGeneratedNever();

            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.UserId);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}