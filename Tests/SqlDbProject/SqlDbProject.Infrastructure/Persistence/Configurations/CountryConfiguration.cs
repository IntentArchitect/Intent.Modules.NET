using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(x => x.CountryIso);

            builder.Property(x => x.CountryIso)
                .HasColumnType("CHAR(2)")
                .ValueGeneratedNever();

            builder.Property(x => x.Description)
                .IsRequired()
                .HasColumnType("nvarchar(64)");

            builder.Property(x => x.CurrencyIso)
                .IsRequired()
                .HasColumnType("CHAR(3)");

            builder.Property(x => x.DialingCode)
                .IsRequired()
                .HasColumnType("nvarchar(8)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}