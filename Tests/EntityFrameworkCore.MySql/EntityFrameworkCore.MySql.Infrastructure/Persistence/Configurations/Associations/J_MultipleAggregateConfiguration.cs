using EntityFrameworkCore.MySql.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.Associations
{
    public class J_MultipleAggregateConfiguration : IEntityTypeConfiguration<J_MultipleAggregate>
    {
        public void Configure(EntityTypeBuilder<J_MultipleAggregate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleAggrAttr)
                .IsRequired();

            builder.Property(x => x.J_RequiredDependentId)
                .IsRequired();

            builder.HasOne(x => x.J_RequiredDependent)
                .WithMany()
                .HasForeignKey(x => x.J_RequiredDependentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}