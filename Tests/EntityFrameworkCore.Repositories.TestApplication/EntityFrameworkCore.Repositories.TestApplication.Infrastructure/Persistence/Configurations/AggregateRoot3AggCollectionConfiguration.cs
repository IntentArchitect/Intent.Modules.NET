using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRoot3AggCollectionConfiguration : IEntityTypeConfiguration<AggregateRoot3AggCollection>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot3AggCollection> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateRoot3SingleId)
                .IsRequired();

            builder.Property(x => x.AggregateRoot3NullableId);

            builder.Property(x => x.AggregateRoot3CollectionId)
                .IsRequired();

            builder.HasOne(x => x.AggregateRoot3Single)
                .WithMany()
                .HasForeignKey(x => x.AggregateRoot3SingleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AggregateRoot3Nullable)
                .WithMany()
                .HasForeignKey(x => x.AggregateRoot3NullableId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AggregateRoot3Collection)
                .WithMany()
                .HasForeignKey(x => x.AggregateRoot3CollectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}