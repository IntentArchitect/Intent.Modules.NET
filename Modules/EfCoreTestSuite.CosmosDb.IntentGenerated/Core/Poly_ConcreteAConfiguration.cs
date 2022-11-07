using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class Poly_ConcreteAConfiguration : IEntityTypeConfiguration<Poly_ConcreteA>
    {
        public void Configure(EntityTypeBuilder<Poly_ConcreteA> builder)
        {
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseField)
                .IsRequired();

            builder.Property(x => x.ConcreteField)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();
        }
    }
}