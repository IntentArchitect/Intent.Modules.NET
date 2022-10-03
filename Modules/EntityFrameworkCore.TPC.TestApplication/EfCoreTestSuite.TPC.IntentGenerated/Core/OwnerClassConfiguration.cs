using System;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core
{
    public class OwnerClassConfiguration : IEntityTypeConfiguration<OwnerClass>
    {
        public void Configure(EntityTypeBuilder<OwnerClass> builder)
        {
            builder.ToTable("OwnerClass");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OwnerField)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);


            builder.OwnsMany(x => x.OwnedClasses, ConfigureOwnedClasses);
        }

        public void ConfigureOwnedClasses(OwnedNavigationBuilder<OwnerClass, OwnedClass> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.OwnerClassId);
            builder.ToTable("OwnedClass");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AbstractField)
                .IsRequired();

            builder.Property(x => x.OwnedField)
                .IsRequired();
        }
    }
}