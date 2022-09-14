using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class BaseConfiguration : IEntityTypeConfiguration<Base>
    {
        public void Configure(EntityTypeBuilder<Base> builder)
        {
            builder.ToContainer("EntityFrameworkCore.CosmosDb.TestApplication");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseField1)
                .IsRequired();


            builder.HasOne(x => x.BaseAssociated)
                .WithMany()
                .HasForeignKey(x => x.BaseAssociatedId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}