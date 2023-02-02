using System;
using EfCoreTestSuite.TPT.IntentGenerated.Entities;
using EfCoreTestSuite.TPT.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core.InheritanceAssociations
{
    public class ConcreteBaseClassAssociatedConfiguration : IEntityTypeConfiguration<ConcreteBaseClassAssociated>
    {
        public void Configure(EntityTypeBuilder<ConcreteBaseClassAssociated> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.HasOne(x => x.ConcreteBaseClass)
                .WithMany()
                .HasForeignKey(x => x.ConcreteBaseClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}