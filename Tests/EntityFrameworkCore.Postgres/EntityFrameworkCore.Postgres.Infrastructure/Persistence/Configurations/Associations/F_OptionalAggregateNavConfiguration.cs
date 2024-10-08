using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations
{
    public class F_OptionalAggregateNavConfiguration : IEntityTypeConfiguration<F_OptionalAggregateNav>
    {
        public void Configure(EntityTypeBuilder<F_OptionalAggregateNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OptionalAggrNavAttr)
                .IsRequired();

            builder.Property(x => x.F_OptionalDependentId);

            builder.HasOne(x => x.F_OptionalDependent)
                .WithOne(x => x.F_OptionalAggregateNav)
                .HasForeignKey<F_OptionalAggregateNav>(x => x.F_OptionalDependentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}