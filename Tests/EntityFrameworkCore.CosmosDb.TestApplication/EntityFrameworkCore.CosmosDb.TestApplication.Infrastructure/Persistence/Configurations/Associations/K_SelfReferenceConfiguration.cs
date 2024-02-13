using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class K_SelfReferenceConfiguration : IEntityTypeConfiguration<K_SelfReference>
    {
        public void Configure(EntityTypeBuilder<K_SelfReference> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.SelfRefAttr)
                .IsRequired();

            builder.Property(x => x.KSelfreferencesId);

            builder.HasOne(x => x.K_SelfReferenceAssociation)
                .WithMany()
                .HasForeignKey(x => x.KSelfreferencesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}