using System;
using EfCoreTestSuite.TPH.IntentGenerated.Entities;
using EfCoreTestSuite.TPH.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Core.InheritanceAssociations
{
    public class FkBaseClassConfiguration : IEntityTypeConfiguration<FkBaseClass>
    {
        public void Configure(EntityTypeBuilder<FkBaseClass> builder)
        {
            builder.HasKey(x => new { x.CompositeKeyA, x.CompositeKeyB });

            builder.Ignore(e => e.DomainEvents);
        }
    }
}