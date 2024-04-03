using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Schema2;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Schema2
{
    public class BanksConfiguration : IEntityTypeConfiguration<Banks>
    {
        public void Configure(EntityTypeBuilder<Banks> builder)
        {
            builder.ToTable("Banks", "schema2");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}