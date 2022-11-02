using System;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations
{
    public class A_Aggregation_ManyConfiguration : IEntityTypeConfiguration<A_Aggregation_Many>
    {
        public void Configure(EntityTypeBuilder<A_Aggregation_Many> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregationAttr)
                .IsRequired();

            builder.Property(x => x.AAggregaterootId);

            builder.OwnsOne(x => x.Composite, ConfigureComposite);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);

            builder.HasOne(x => x.Aggregation)
                .WithOne()
                .HasForeignKey<A_Aggregation_Many>(x => x.AggregationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Aggregations)
                .WithOne()
                .HasForeignKey(x => x.A_Aggregation_ManyId);
        }

        public void ConfigureComposite(OwnedNavigationBuilder<A_Aggregation_Many, AA4_Composite_Single> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }

        public void ConfigureComposites(OwnedNavigationBuilder<A_Aggregation_Many, AA4_Composite_Many> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.A_Aggregation_ManyId);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.AAggregationManyId)
                .IsRequired();
        }
    }
}