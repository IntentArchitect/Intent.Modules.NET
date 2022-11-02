using System;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations
{
    public class A_AggregateRootConfiguration : IEntityTypeConfiguration<A_AggregateRoot>
    {
        public void Configure(EntityTypeBuilder<A_AggregateRoot> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateAttr)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);

            builder.HasOne(x => x.Aggregation)
                .WithOne()
                .HasForeignKey<A_AggregateRoot>(x => x.AggregationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Aggregations)
                .WithOne()
                .HasForeignKey(x => x.A_AggregateRootId);
        }

        public void ConfigureComposite(OwnedNavigationBuilder<A_Composite_Single, AA1_Composite_Single> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<A_Composite_Single, AA1_Composite_Many> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_Composite_SingleId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.ACompositeSingleId)
                .IsRequired();
        }

        public void ConfigureComposite(OwnedNavigationBuilder<A_AggregateRoot, A_Composite_Single> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);

            builder.HasOne(x => x.Aggregation)
                .WithOne()
                .HasForeignKey<A_Composite_Single>(x => x.AggregationId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void ConfigureComposite(OwnedNavigationBuilder<A_Composite_Many, AA2_Composite_Single> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<A_Composite_Many, AA2_Composite_Many> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_Composite_ManyId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.ACompositeManyId)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<A_AggregateRoot, A_Composite_Many> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_AggregateRootId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AAggregaterootId)
                .IsRequired();

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);

            builder.HasOne(x => x.Aggregation)
                .WithOne()
                .HasForeignKey<A_Composite_Many>(x => x.AggregationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}