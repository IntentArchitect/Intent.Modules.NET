using System;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core.Associations
{
    public class C_RequiredCompositeConfiguration : IEntityTypeConfiguration<C_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<C_RequiredComposite> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompAttr)
                .IsRequired();

            builder.OwnsMany(x => x.C_MultipleDependents, ConfigureC_MultipleDependents);
        }

        public void ConfigureC_MultipleDependents(OwnedNavigationBuilder<C_RequiredComposite, C_MultipleDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.C_RequiredCompositeId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleDepAttr)
                .IsRequired();
        }
    }
}