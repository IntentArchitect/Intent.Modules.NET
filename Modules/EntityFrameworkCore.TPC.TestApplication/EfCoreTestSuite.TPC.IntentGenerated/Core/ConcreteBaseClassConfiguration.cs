using System;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core
{
    public class ConcreteBaseClassConfiguration : IEntityTypeConfiguration<ConcreteBaseClass>
    {
        public void Configure(EntityTypeBuilder<ConcreteBaseClass> builder)
        {
            builder.ToTable("ConcreteBaseClass");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}