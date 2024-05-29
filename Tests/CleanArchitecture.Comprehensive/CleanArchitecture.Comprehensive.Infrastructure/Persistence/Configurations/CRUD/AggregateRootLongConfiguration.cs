using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.CRUD
{
    public class AggregateRootLongConfiguration : IEntityTypeConfiguration<AggregateRootLong>
    {
        public void Configure(EntityTypeBuilder<AggregateRootLong> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.OwnsOne(x => x.CompositeOfAggrLong, ConfigureCompositeOfAggrLong);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureCompositeOfAggrLong(OwnedNavigationBuilder<AggregateRootLong, CompositeOfAggrLong> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}