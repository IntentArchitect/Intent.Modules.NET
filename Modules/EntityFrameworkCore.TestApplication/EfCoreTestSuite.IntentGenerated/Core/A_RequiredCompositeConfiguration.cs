using System;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class A_RequiredCompositeConfiguration : IEntityTypeConfiguration<A_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<A_RequiredComposite> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.A_OptionalDependent)
                .WithOne()
                .HasForeignKey<A_OptionalDependent>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}