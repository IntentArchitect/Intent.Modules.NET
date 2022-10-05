using System;
using EfCoreTestSuite.TPH.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Core
{
    public class DerivedClassForAbstractAssociatedConfiguration : IEntityTypeConfiguration<DerivedClassForAbstractAssociated>
    {
        public void Configure(EntityTypeBuilder<DerivedClassForAbstractAssociated> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);


            builder.HasOne(x => x.DerivedClassForAbstract)
                .WithMany()
                .HasForeignKey(x => x.DerivedClassForAbstractId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}