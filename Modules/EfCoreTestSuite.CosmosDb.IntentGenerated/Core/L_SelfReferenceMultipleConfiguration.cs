using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class L_SelfReferenceMultipleConfiguration : IEntityTypeConfiguration<L_SelfReferenceMultiple>
    {
        public void Configure(EntityTypeBuilder<L_SelfReferenceMultiple> builder)
        {
            builder.ToTable("L_SelfReferenceMultiple");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.SelfRefMulAttr)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasMany(x => x.L_SelfReferenceMultiplesDst)
                .WithMany("L_SelfReferenceMultiplesSrc")
                .UsingEntity(x => x.ToTable("L_SelfReferenceMultipleL_SelfReferenceMultiples"));
        }
    }
}