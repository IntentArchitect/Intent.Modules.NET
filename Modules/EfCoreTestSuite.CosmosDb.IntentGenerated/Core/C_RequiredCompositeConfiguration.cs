using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class C_RequiredCompositeConfiguration : IEntityTypeConfiguration<C_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<C_RequiredComposite> builder)
        {
            builder.ToTable("C_RequiredComposite");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.RequiredCompositeAttr)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.OwnsMany(x => x.C_MultipleDependents, ConfigureC_MultipleDependents);

        }

        public void ConfigureC_MultipleDependents(OwnedNavigationBuilder<C_RequiredComposite, C_MultipleDependent> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.C_RequiredCompositeId);
            builder.ToTable("C_MultipleDependent");


            builder.Property(x => x.MultipleDependentAttr)
                .IsRequired();
        }
    }
}