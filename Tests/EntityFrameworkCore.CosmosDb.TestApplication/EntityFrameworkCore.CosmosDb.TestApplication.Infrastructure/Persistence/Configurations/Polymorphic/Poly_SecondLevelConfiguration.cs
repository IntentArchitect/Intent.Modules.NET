using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Polymorphic
{
    public class Poly_SecondLevelConfiguration : IEntityTypeConfiguration<Poly_SecondLevel>
    {
        public void Configure(EntityTypeBuilder<Poly_SecondLevel> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SecondField)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.HasOne(x => x.BaseClassNonAbstracts)
                .WithOne()
                .HasForeignKey<Poly_BaseClassNonAbstract>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}