using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class C_RequiredCompositeConfiguration : IEntityTypeConfiguration<C_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<C_RequiredComposite> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.RequiredCompositeAttr)
                .IsRequired();

            builder.OwnsMany(x => x.C_MultipleDependents, ConfigureC_MultipleDependents);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureC_MultipleDependents(OwnedNavigationBuilder<C_RequiredComposite, C_MultipleDependent> builder)
        {
            builder.WithOwner();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleDependentAttr)
                .IsRequired();

            builder.Property(x => x.CRequiredcompositeId)
                .IsRequired();
        }
    }
}