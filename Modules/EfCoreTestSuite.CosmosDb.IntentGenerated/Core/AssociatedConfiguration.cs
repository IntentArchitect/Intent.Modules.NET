using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class AssociatedConfiguration : IEntityTypeConfiguration<Associated>
    {
        public void Configure(EntityTypeBuilder<Associated> builder)
        {
            builder.ToTable("Associated");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.AssociatedField1)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

        }
    }
}