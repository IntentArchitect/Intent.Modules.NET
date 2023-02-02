using System;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core.ExplicitKeys
{
    public class FK_B_CompositeForeignKeyConfiguration : IEntityTypeConfiguration<FK_B_CompositeForeignKey>
    {
        public void Configure(EntityTypeBuilder<FK_B_CompositeForeignKey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PKCompositeKeyCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.PKCompositeKeyCompositeKeyB)
                .IsRequired();

            builder.HasOne(x => x.PKCompositeKey)
                .WithMany()
                .HasForeignKey(x => new { x.PKCompositeKeyCompositeKeyA, x.PKCompositeKeyCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}