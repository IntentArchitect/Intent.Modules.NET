using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class ConcreteBaseClassConfiguration : IEntityTypeConfiguration<ConcreteBaseClass>
    {
        public void Configure(EntityTypeBuilder<ConcreteBaseClass> builder)
        {
            builder.ToContainer("EntityFrameworkCore.CosmosDb.TestApplication");

            builder.HasKey(x => new { x.PartitionKey, x.Id });

            builder.Property(x => x.BaseAttribute)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.PartitionKey)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);
        }
    }
}