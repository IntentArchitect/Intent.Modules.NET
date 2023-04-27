using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.CRUD
{
    public class AggregateRootConfiguration : IEntityTypeConfiguration<AggregateRoot>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateAttr)
                .IsRequired();

            builder.HasMany(x => x.Composites)
                .WithOne()
                .HasForeignKey(x => x.AggregateRootId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Composite)
                .WithOne()
                .HasForeignKey<CompositeSingleA>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Aggregate)
                .WithOne()
                .HasForeignKey<AggregateRoot>(x => x.AggregateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}