using System;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class M_SelfReferenceBiNavConfiguration : IEntityTypeConfiguration<M_SelfReferenceBiNav>
    {
        public void Configure(EntityTypeBuilder<M_SelfReferenceBiNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SelfRefBiNavAttr)
                .IsRequired();

            builder.HasOne(x => x.MSelfReferenceBiNavDst)
                .WithMany(x => x.MSelfReferenceBiNavs)
                .HasForeignKey(x => x.MSelfReferenceBiNavDstId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}