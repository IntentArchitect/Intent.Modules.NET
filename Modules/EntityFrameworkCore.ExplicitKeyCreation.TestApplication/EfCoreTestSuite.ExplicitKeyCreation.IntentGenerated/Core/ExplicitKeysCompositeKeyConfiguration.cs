using System;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Core
{
    public class ExplicitKeysCompositeKeyConfiguration : IEntityTypeConfiguration<ExplicitKeysCompositeKey>
    {
        public void Configure(EntityTypeBuilder<ExplicitKeysCompositeKey> builder)
        {
            builder.HasKey(x => new { x.CompositeKeyA, x.CompositeKeyB });
        }
    }
}