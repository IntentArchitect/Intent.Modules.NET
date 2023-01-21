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
    public class K_SelfReferenceConfiguration : IEntityTypeConfiguration<K_SelfReference>
    {
        public void Configure(EntityTypeBuilder<K_SelfReference> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SelfRefAttr)
                .IsRequired();

            builder.HasOne(x => x.KSelfReferenceAssociation)
                .WithMany()
                .HasForeignKey(x => x.KSelfReferenceAssociationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}