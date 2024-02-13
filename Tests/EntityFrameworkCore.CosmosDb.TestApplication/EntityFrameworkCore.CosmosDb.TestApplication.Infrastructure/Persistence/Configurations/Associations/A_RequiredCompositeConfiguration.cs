using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class A_RequiredCompositeConfiguration : IEntityTypeConfiguration<A_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<A_RequiredComposite> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompositeAttr)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.OwnsOne(x => x.A_OptionalDependent, ConfigureA_OptionalDependent);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureA_OptionalDependent(OwnedNavigationBuilder<A_RequiredComposite, A_OptionalDependent> builder)
        {
            builder.WithOwner();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OptionalDependentAttr)
                .IsRequired();
        }
    }
}