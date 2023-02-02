using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core.InheritanceAssociations
{
    public class DerivedClassForConcreteAssociatedConfiguration : IEntityTypeConfiguration<DerivedClassForConcreteAssociated>
    {
        public void Configure(EntityTypeBuilder<DerivedClassForConcreteAssociated> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.HasOne(x => x.DerivedClassForConcrete)
                .WithMany()
                .HasForeignKey(x => x.DerivedClassForConcreteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}