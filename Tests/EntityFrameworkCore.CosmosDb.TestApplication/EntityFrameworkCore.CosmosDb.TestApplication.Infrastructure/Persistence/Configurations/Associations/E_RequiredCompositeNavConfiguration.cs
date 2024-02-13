using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class E_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<E_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<E_RequiredCompositeNav> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompositeNavAttr)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.OwnsOne(x => x.E_RequiredDependent, ConfigureE_RequiredDependent)
                .Navigation(x => x.E_RequiredDependent).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureE_RequiredDependent(OwnedNavigationBuilder<E_RequiredCompositeNav, E_RequiredDependent> builder)
        {
            builder.WithOwner(x => x.E_RequiredCompositeNav);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredDependentAttr)
                .IsRequired();
        }
    }
}