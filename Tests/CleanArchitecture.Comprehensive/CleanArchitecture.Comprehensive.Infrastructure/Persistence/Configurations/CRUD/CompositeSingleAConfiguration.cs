using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.CRUD
{
    public class CompositeSingleAConfiguration : IEntityTypeConfiguration<CompositeSingleA>
    {
        public void Configure(EntityTypeBuilder<CompositeSingleA> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.HasOne(x => x.Composite)
                .WithOne()
                .HasForeignKey<CompositeSingleAA>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(x => x.Composites, ConfigureComposites);
        }

        public void ConfigureComposites(OwnedNavigationBuilder<CompositeSingleA, CompositeManyAA> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CompositeSingleAId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.CompositeSingleAId)
                .IsRequired();
        }
    }
}