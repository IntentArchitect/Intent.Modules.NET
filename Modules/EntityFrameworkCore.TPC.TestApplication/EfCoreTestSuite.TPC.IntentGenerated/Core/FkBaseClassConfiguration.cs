using System;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core
{
    public class FkBaseClassConfiguration : IEntityTypeConfiguration<FkBaseClass>
    {
        public void Configure(EntityTypeBuilder<FkBaseClass> builder)
        {
            builder.ToTable("FkBaseClass");

            builder.HasKey(x => new { x.CompositeKeyA, x.CompositeKeyB });
        }
    }
}