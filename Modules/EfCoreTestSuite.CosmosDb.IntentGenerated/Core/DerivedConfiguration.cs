using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance;
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
            builder.ToContainer("EntityFrameworkCore.CosmosDb.TestApplication");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseField1)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.DerivedField1)
                .IsRequired();

            builder.HasOne(x => x.BaseAssociated)
                .WithMany()
                .HasForeignKey(x => x.BaseAssociatedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Associated)
                .WithMany()
                .HasForeignKey(x => x.AssociatedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);
        }

        public void ConfigureComposites(OwnedNavigationBuilder<Derived, Composite> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.DerivedId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeField1)
                .IsRequired();
        }
    }
}