using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class J_MultipleAggregateConfiguration : IEntityTypeConfiguration<J_MultipleAggregate>
    {
        public void Configure(EntityTypeBuilder<J_MultipleAggregate> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.MultipleAggrAttr)
                .IsRequired();

            builder.Property(x => x.JRequireddependentId)
                .IsRequired();

            builder.HasOne(x => x.J_RequiredDependent)
                .WithMany()
                .HasForeignKey(x => x.JRequireddependentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}