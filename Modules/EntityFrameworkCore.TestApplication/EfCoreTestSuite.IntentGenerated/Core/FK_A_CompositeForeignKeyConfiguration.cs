using System;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class FK_A_CompositeForeignKeyConfiguration : IEntityTypeConfiguration<FK_A_CompositeForeignKey>
    {
        public void Configure(EntityTypeBuilder<FK_A_CompositeForeignKey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ForeignCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.ForeignCompositeKeyB)
                .IsRequired();


            builder.HasOne(x => x.PK_CompositeKey)
            .WithMany()
            .HasForeignKey(x => new { x.ForeignCompositeKeyA, x.ForeignCompositeKeyB })
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}