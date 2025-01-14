using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure.Persistence.Configurations
{
    public class CheckNewCompChildCrudConfiguration : IEntityTypeConfiguration<CheckNewCompChildCrud>
    {
        public void Configure(EntityTypeBuilder<CheckNewCompChildCrud> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.CNCCChildren, ConfigureCNCCChildren);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureCNCCChildren(OwnedNavigationBuilder<CheckNewCompChildCrud, CNCCChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CheckNewCompChildCrudId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.CheckNewCompChildCrudId)
                .IsRequired();
        }
    }
}