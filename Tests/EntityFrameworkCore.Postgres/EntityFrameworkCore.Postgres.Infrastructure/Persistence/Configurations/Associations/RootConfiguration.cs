using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations
{
    public class RootConfiguration : IEntityTypeConfiguration<Root>
    {
        public void Configure(EntityTypeBuilder<Root> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsOne(x => x.ChildNoPK, ConfigureChildNoPK)
                .Navigation(x => x.ChildNoPK).IsRequired();
        }

        public static void ConfigureChildNoPK(OwnedNavigationBuilder<Root, ChildNoPK> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => new { });

            builder.Property(x => x.Age)
                .IsRequired();
        }
    }
}