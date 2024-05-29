using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.CRUD
{
    public class CompositeManyBConfiguration : IEntityTypeConfiguration<CompositeManyB>
    {
        public void Configure(EntityTypeBuilder<CompositeManyB> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.SomeDate);

            builder.Property(x => x.AggregateRootId)
                .IsRequired();

            builder.HasOne(x => x.Composite)
                .WithOne()
                .HasForeignKey<CompositeSingleBB>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);
        }

        public void ConfigureComposites(OwnedNavigationBuilder<CompositeManyB, CompositeManyBB> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CompositeManyBId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.CompositeManyBId)
                .IsRequired();
        }
    }
}