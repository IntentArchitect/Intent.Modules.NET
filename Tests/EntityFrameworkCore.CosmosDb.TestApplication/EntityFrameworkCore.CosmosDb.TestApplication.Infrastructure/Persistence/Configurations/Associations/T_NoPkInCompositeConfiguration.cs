using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class T_NoPkInCompositeConfiguration : IEntityTypeConfiguration<T_NoPkInComposite>
    {
        public void Configure(EntityTypeBuilder<T_NoPkInComposite> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.OwnsOne(x => x.T_NoPkInCompositeDependent, ConfigureT_NoPkInCompositeDependent);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureT_NoPkInCompositeDependent(OwnedNavigationBuilder<T_NoPkInComposite, T_NoPkInCompositeDependent> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Description)
                .IsRequired();
        }
    }
}