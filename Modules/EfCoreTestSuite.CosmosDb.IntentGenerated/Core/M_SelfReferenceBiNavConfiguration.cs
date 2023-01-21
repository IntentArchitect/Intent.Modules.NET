using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class M_SelfReferenceBiNavConfiguration : IEntityTypeConfiguration<M_SelfReferenceBiNav>
    {
        public void Configure(EntityTypeBuilder<M_SelfReferenceBiNav> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.SelfRefBiNavAttr)
                .IsRequired();

            builder.HasOne(x => x.MSelfReferenceBiNavAssocation)
                .WithMany(x => x.MSelfReferenceBiNavs)
                .HasForeignKey(x => x.MSelfReferenceBiNavAssocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}