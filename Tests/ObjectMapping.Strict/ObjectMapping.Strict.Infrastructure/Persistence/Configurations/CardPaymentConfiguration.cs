using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObjectMapping.Strict.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace ObjectMapping.Strict.Infrastructure.Persistence.Configurations
{
    public class CardPaymentConfiguration : IEntityTypeConfiguration<CardPayment>
    {
        public void Configure(EntityTypeBuilder<CardPayment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Label)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.CardLast4)
                .IsRequired();
        }
    }
}