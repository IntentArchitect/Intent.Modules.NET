using Finbuckle.MultiTenant.EntityFrameworkCore;
using Finbuckle.SharedDatabase.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.Username)
                .IsRequired();

            builder.OwnsMany(x => x.Roles, ConfigureRoles);

            builder.IsMultiTenant();
        }

        public void ConfigureRoles(OwnedNavigationBuilder<User, Role> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.UserId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();
        }
    }
}