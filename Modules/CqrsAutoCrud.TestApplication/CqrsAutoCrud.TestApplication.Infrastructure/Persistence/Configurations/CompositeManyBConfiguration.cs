using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations
{
    public class CompositeManyBConfiguration : IEntityTypeConfiguration<CompositeManyB>
    {
        public void Configure(EntityTypeBuilder<CompositeManyB> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.AAggregaterootId)
                .IsRequired();

            builder.Property(x => x.SomeDate);

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
                .HasForeignKey(x => x.A_Composite_ManyId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();

            builder.Property(x => x.ACompositeManyId)
                .IsRequired();
        }
    }
}