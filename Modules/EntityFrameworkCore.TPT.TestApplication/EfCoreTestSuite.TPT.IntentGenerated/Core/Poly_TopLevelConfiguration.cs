using System;
using EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core
{
    public class Poly_TopLevelConfiguration : IEntityTypeConfiguration<Poly_TopLevel>
    {
        public void Configure(EntityTypeBuilder<Poly_TopLevel> builder)
        {
            builder.ToTable("Poly_TopLevel");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TopField)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);


            builder.HasMany(x => x.RootAbstracts)
                .WithOne()
                .HasForeignKey(x => x.TopLevelId);
        }
    }
}