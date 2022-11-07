using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class Poly_SecondLevelConfiguration : IEntityTypeConfiguration<Poly_SecondLevel>
    {
        public void Configure(EntityTypeBuilder<Poly_SecondLevel> builder)
        {
            builder.ToTable("Poly_SecondLevel");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SecondField)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.OwnsOne(x => x.BaseClassNonAbstracts, ConfigureBaseClassNonAbstracts)
                .Navigation(x => x.BaseClassNonAbstracts).IsRequired();
        }

        public void ConfigureBaseClassNonAbstracts(OwnedNavigationBuilder<Poly_SecondLevel, Poly_BaseClassNonAbstract> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("Poly_BaseClassNonAbstract");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseField)
                .IsRequired();
        }
    }
}