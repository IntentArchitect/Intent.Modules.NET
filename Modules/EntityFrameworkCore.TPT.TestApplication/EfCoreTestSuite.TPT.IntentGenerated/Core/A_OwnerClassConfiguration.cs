using System;
using EfCoreTestSuite.TPT.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core
{
    public class A_OwnerClassConfiguration : IEntityTypeConfiguration<A_OwnerClass>
    {
        public void Configure(EntityTypeBuilder<A_OwnerClass> builder)
        {
            builder.ToTable("A_OwnerClass");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OwnerField)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);


            builder.OwnsMany(x => x.A_OwnedClasses, ConfigureA_OwnedClasses);
        }

        public void ConfigureA_OwnedClasses(OwnedNavigationBuilder<A_OwnerClass, A_OwnedClass> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_OwnerClassId);
            builder.ToTable("A_OwnedClass");

            builder.Property(x => x.OwnedField)
                .IsRequired();
        }
    }
}