using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRoot4AggNullableConfiguration : IEntityTypeConfiguration<AggregateRoot4AggNullable>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot4AggNullable> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateRoot4SingleId)
                .IsRequired();

            builder.Property(x => x.AggregateRoot4NullableId);

            builder.HasOne(x => x.AggregateRoot4Single)
                .WithOne()
                .HasForeignKey<AggregateRoot4AggNullable>(x => x.AggregateRoot4SingleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.AggregateRoot4Collections)
                .WithOne()
                .HasForeignKey(x => x.AggregateRoot4AggNullableId);

            builder.HasOne(x => x.AggregateRoot4Nullable)
                .WithOne()
                .HasForeignKey<AggregateRoot4AggNullable>(x => x.AggregateRoot4NullableId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}