using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class DerivedConfiguration : IEntityTypeConfiguration<Derived>
    {
        public void Configure(EntityTypeBuilder<Derived> builder)
        {
            builder.HasBaseType<Base>();


            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.DerivedField1)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasOne(x => x.Associated)
                .WithMany()
                .HasForeignKey(x => x.AssociatedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);

        }

        public void ConfigureComposites(OwnedNavigationBuilder<Derived, Composite> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.DerivedId);

            builder.Property(x => x.CompositeField1)
                .IsRequired();
        }
    }
}