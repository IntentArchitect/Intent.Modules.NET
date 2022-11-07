using System;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRootConfiguration : IEntityTypeConfiguration<AggregateRoot>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateAttr)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);

            builder.HasOne(x => x.Aggregate)
                .WithOne()
                .HasForeignKey<AggregateRoot>(x => x.AggregateId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void ConfigureComposite(OwnedNavigationBuilder<CompositeSingleA, CompositeSingleAA> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("CompositeSingleAA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<CompositeSingleA, CompositeManyAA> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_Composite_SingleId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.ACompositeSingleId)
                .IsRequired();
        }

        public void ConfigureComposite(OwnedNavigationBuilder<AggregateRoot, CompositeSingleA> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("CompositeSingleA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);
        }

        public void ConfigureComposite(OwnedNavigationBuilder<CompositeManyB, CompositeSingleBB> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.ToTable("CompositeSingleBB");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<CompositeManyB, CompositeManyBB> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_Composite_ManyId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.ACompositeManyId)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<AggregateRoot, CompositeManyB> builder)
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