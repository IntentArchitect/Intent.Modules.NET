using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class ClassAConfiguration : IEntityTypeConfiguration<ClassA>
    {
        public void Configure(EntityTypeBuilder<ClassA> builder)
        {
            builder.ToTable("ClassA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.ClassAAttr)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.OwnsMany(x => x.ClassBS, ConfigureClassBS);
        }

        public void ConfigureClassC(OwnedNavigationBuilder<ClassB, ClassC> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("ClassC");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClassCAttr)
                .IsRequired();
        }

        public void ConfigureClassE(OwnedNavigationBuilder<ClassD, ClassE> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("ClassE");

            builder.HasKey(x => x.Id);
        }

        public void ConfigureClassDS(OwnedNavigationBuilder<ClassB, ClassD> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.ClassBId);
            builder.ToTable("ClassD");

            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.ClassE, ConfigureClassE)
                .Navigation(x => x.ClassE).IsRequired();
        }

        public void ConfigureClassBS(OwnedNavigationBuilder<ClassA, ClassB> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.ClassAId);
            builder.ToTable("ClassB");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClassBAttr)
                .IsRequired();

            builder.OwnsOne(x => x.ClassC, ConfigureClassC)
                .Navigation(x => x.ClassC).IsRequired();

            builder.OwnsMany(x => x.ClassDS, ConfigureClassDS);
        }
    }
}