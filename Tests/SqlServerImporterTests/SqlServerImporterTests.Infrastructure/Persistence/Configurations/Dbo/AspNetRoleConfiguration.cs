using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class AspNetRoleConfiguration : IEntityTypeConfiguration<AspNetRole>
    {
        public void Configure(EntityTypeBuilder<AspNetRole> builder)
        {
            builder.ToTable("AspNetRoles", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar(256)");

            builder.Property(x => x.NormalizedName)
                .HasColumnType("nvarchar(256)");

            builder.Property(x => x.ConcurrencyStamp)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => x.NormalizedName)
                .IsUnique()
                .HasDatabaseName("RoleNameIndex");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}