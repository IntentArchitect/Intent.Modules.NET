using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class AspNetUserConfiguration : IEntityTypeConfiguration<AspNetUser>
    {
        public void Configure(EntityTypeBuilder<AspNetUser> builder)
        {
            builder.ToTable("AspNetUsers", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefreshToken)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RefreshTokenExpired);

            builder.Property(x => x.UserName)
                .HasColumnType("nvarchar(256)");

            builder.Property(x => x.NormalizedUserName)
                .HasColumnType("nvarchar(256)");

            builder.Property(x => x.Email)
                .HasColumnType("nvarchar(256)");

            builder.Property(x => x.NormalizedEmail)
                .HasColumnType("nvarchar(256)");

            builder.Property(x => x.EmailConfirmed)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.SecurityStamp)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ConcurrencyStamp)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.PhoneNumber)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.PhoneNumberConfirmed)
                .IsRequired();

            builder.Property(x => x.TwoFactorEnabled)
                .IsRequired();

            builder.Property(x => x.LockoutEnd);

            builder.Property(x => x.LockoutEnabled)
                .IsRequired();

            builder.Property(x => x.AccessFailedCount)
                .IsRequired();

            builder.HasIndex(x => x.NormalizedEmail)
                .HasDatabaseName("EmailIndex");

            builder.HasIndex(x => x.NormalizedUserName)
                .IsUnique()
                .HasDatabaseName("UserNameIndex");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}