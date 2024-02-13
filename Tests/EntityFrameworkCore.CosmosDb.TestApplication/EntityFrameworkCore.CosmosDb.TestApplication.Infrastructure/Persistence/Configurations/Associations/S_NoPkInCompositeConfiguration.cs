using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class S_NoPkInCompositeConfiguration : IEntityTypeConfiguration<S_NoPkInComposite>
    {
        public void Configure(EntityTypeBuilder<S_NoPkInComposite> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.OwnsOne(x => x.S_NoPkInCompositeDependent, ConfigureS_NoPkInCompositeDependent)
                .Navigation(x => x.S_NoPkInCompositeDependent).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureS_NoPkInCompositeDependent(OwnedNavigationBuilder<S_NoPkInComposite, S_NoPkInCompositeDependent> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Description)
                .IsRequired();
        }
    }
}