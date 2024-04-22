using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class ParentConfiguration : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            builder.ToTable("Parents", "dbo");

            builder.HasKey(x => new { x.Id, x.Id2 });

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}