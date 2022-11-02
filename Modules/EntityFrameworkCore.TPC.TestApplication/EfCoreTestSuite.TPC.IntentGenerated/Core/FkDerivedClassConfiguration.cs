using System;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using EfCoreTestSuite.TPC.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core
{
    public class FkDerivedClassConfiguration : IEntityTypeConfiguration<FkDerivedClass>
    {
        public void Configure(EntityTypeBuilder<FkDerivedClass> builder)
        {
            builder.ToTable("FkDerivedClasses");

            builder.Property(x => x.DerivedField)
                .IsRequired();
        }
    }
}