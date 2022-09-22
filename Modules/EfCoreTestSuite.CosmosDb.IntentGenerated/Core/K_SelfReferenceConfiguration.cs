using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class K_SelfReferenceConfiguration : IEntityTypeConfiguration<K_SelfReference>
    {
        public void Configure(EntityTypeBuilder<K_SelfReference> builder)
        {
            builder.ToTable("K_SelfReference");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.SelfRefAttr)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasOne(x => x.K_SelfReferenceAssociation)
                .WithMany()
                .HasForeignKey(x => x.K_SelfReferenceAssociationId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}