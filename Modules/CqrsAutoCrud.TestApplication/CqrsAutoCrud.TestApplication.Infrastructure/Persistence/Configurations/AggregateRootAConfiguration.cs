using System;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRootAConfiguration : IEntityTypeConfiguration<AggregateRootA>
    {
        public void Configure(EntityTypeBuilder<AggregateRootA> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateAttr)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);

            builder.HasOne(x => x.Aggregate)
                .WithOne()
                .HasForeignKey<AggregateRootA>(x => x.AggregateId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void ConfigureComposite(OwnedNavigationBuilder<CompositeSingleAA, CompositeSingleAAA1> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("CompositeSingleAAA1");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<CompositeSingleAA, CompositeManyAAA1> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_Composite_SingleId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.ACompositeSingleId)
                .IsRequired();
        }

        public void ConfigureComposite(OwnedNavigationBuilder<AggregateRootA, CompositeSingleAA> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("CompositeSingleAA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);
        }

        public void ConfigureComposite(OwnedNavigationBuilder<CompositeManyAA, CompositeSingleAAA2> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("CompositeSingleAAA2");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<CompositeManyAA, CompositeManyAAA2> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_Composite_ManyId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.ACompositeManyId)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<AggregateRootA, CompositeManyAA> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_AggregateRootId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.AAggregaterootId)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);
        }
    }
}