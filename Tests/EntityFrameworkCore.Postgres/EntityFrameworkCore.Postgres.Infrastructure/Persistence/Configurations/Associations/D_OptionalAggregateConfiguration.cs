using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations
{
    public class D_OptionalAggregateConfiguration : IEntityTypeConfiguration<D_OptionalAggregate>
    {
        public void Configure(EntityTypeBuilder<D_OptionalAggregate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OptionalAggrAttr)
                .IsRequired();

            builder.HasMany(x => x.D_MultipleDependents)
                .WithOne()
                .HasForeignKey(x => x.D_OptionalAggregateId);
        }
    }
}