using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.Priviledges, ConfigurePriviledges);
        }

        public void ConfigurePriviledges(OwnedNavigationBuilder<Role, Priviledge> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.RoleId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoleId)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}