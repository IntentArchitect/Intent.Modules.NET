using System;
using EfCoreTestSuite.IntentGenerated.Entities;
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


            builder.HasOne(x => x.M_SelfReferenceBiNavDst)
                .WithMany(x => x.M_SelfReferenceBiNavs)
                .HasForeignKey(x => x.M_SelfReferenceBiNavDstId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}