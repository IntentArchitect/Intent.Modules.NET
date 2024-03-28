using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class AspNetUserRoleConfiguration : IEntityTypeConfiguration<AspNetUserRole>
    {
        public void Configure(EntityTypeBuilder<AspNetUserRole> builder)
        {
            builder.ToTable("AspNetUserRoles", "dbo");

            builder.HasKey(x => new { x.UserId, x.RoleId });

            builder.HasOne(x => x.RoleIdAspNetRoles)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserIdAspNetUsers)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}