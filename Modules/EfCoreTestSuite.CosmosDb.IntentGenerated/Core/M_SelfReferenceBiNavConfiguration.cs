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
            builder.ToContainer("EntityFrameworkCore.CosmosDb.TestApplication");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.SelfRefBiNavAttr)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasOne(x => x.M_SelfReferenceBiNavAssocation)
                .WithMany(x => x.M_SelfReferenceBiNavs)
                .HasForeignKey(x => x.M_SelfReferenceBiNavAssocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}