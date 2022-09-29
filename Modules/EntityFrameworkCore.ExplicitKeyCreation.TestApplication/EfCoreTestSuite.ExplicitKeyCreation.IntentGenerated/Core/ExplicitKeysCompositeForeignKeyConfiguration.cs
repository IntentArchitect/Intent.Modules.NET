using System;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Core
{
    public class ExplicitKeysCompositeForeignKeyConfiguration : IEntityTypeConfiguration<ExplicitKeysCompositeForeignKey>
    {
        public void Configure(EntityTypeBuilder<ExplicitKeysCompositeForeignKey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExplicitKeysCompositeKeyCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.ExplicitKeysCompositeKeyCompositeKeyB)
                .IsRequired();

            builder.HasOne(x => x.ExplicitKeysCompositeKey)
                .WithMany()
                .HasForeignKey(x => new { x.ExplicitKeysCompositeKeyCompositeKeyA, x.ExplicitKeysCompositeKeyCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}