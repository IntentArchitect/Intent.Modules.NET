using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRoot2CompositionConfiguration : IEntityTypeConfiguration<AggregateRoot2Composition>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot2Composition> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.AggregateRoot2Single, ConfigureAggregateRoot2Single)
                .Navigation(x => x.AggregateRoot2Single).IsRequired();

            builder.OwnsOne(x => x.AggregateRoot2Nullable, ConfigureAggregateRoot2Nullable);

            builder.OwnsMany(x => x.AggregateRoot2Collections, ConfigureAggregateRoot2Collections);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAggregateRoot2Single(OwnedNavigationBuilder<AggregateRoot2Composition, AggregateRoot2Single> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);
        }

        public void ConfigureAggregateRoot2Nullable(OwnedNavigationBuilder<AggregateRoot2Composition, AggregateRoot2Nullable> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);
        }

        public void ConfigureAggregateRoot2Collections(OwnedNavigationBuilder<AggregateRoot2Composition, AggregateRoot2Collection> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.AggregateRoot2CompositionId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateRoot2CompositionId)
                .IsRequired();
        }
    }
}