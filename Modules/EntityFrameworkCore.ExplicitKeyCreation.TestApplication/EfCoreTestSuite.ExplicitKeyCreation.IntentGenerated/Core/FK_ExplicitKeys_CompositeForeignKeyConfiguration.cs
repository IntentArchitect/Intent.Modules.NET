using System;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Core
{
    public class FK_ExplicitKeys_CompositeForeignKeyConfiguration : IEntityTypeConfiguration<FK_ExplicitKeys_CompositeForeignKey>
    {
        public void Configure(EntityTypeBuilder<FK_ExplicitKeys_CompositeForeignKey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PkExplicitkeysCompositekeyCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.PkExplicitkeysCompositekeyCompositeKeyB)
                .IsRequired();


            builder.HasOne(x => x.PK_ExplicitKeys_CompositeKey)
                .WithMany()
                .HasForeignKey(x => new { x.PK_ExplicitKeys_CompositeKeyCompositeKeyA, x.PK_ExplicitKeys_CompositeKeyCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}