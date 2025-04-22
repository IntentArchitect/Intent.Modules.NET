using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations.Accounts
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currencies", "finance");

            builder.HasKey(x => x.CurrencyIso);

            builder.Property(x => x.CurrencyIso)
                .ValueGeneratedNever();

            builder.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.AlphaCode)
                .IsRequired()
                .HasColumnType("NCHAR(3)");

            builder.Property(x => x.NumericCode)
                .IsRequired();

            builder.Property(x => x.IsEnabled)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(x => x.Sequence);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}