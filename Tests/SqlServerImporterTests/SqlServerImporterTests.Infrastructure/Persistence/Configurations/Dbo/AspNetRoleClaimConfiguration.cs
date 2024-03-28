using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class AspNetRoleClaimConfiguration : IEntityTypeConfiguration<AspNetRoleClaim>
    {
        public void Configure(EntityTypeBuilder<AspNetRoleClaim> builder)
        {
            builder.ToTable("AspNetRoleClaims", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoleId)
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            builder.Property(x => x.ClaimType)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ClaimValue)
                .HasColumnType("nvarchar(max)");

            builder.HasOne(x => x.RoleIdAspNetRoles)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}