using System;
using EfCoreTestSuite.TPH.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Core
{
    public class DerivedClassConfiguration : IEntityTypeConfiguration<DerivedClass>
    {
        public void Configure(EntityTypeBuilder<DerivedClass> builder)
        {
            builder.HasBaseType<BaseClass>();


            builder.Property(x => x.DerivedAttribute)
                .IsRequired()
                .HasMaxLength(250);


        }
    }
}