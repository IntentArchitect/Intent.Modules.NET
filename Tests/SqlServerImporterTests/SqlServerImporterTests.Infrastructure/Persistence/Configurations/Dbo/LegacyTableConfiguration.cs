using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class LegacyTableConfiguration : IEntityTypeConfiguration<LegacyTable>
    {
        public void Configure(EntityTypeBuilder<LegacyTable> builder)
        {
            builder.ToTable("Legacy_Table", "dbo");

            builder.HasNoKey();

            builder.Property(x => x.LegacyId)
                .IsRequired()
                .HasColumnName("LegacyID");

            builder.Property(x => x.LegacyColumn)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(x => x.BadDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}