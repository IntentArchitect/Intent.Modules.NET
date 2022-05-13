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
    public class E_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<E_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<E_RequiredCompositeNav> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.E_RequiredDependent)
                .WithOne(x => x.E_RequiredCompositeNav)
                .HasForeignKey<E_RequiredCompositeNav>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}