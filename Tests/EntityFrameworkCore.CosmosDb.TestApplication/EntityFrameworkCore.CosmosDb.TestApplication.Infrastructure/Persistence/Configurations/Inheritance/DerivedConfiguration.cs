using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Inheritance
{
    public class DerivedConfiguration : IEntityTypeConfiguration<Derived>
    {
        public void Configure(EntityTypeBuilder<Derived> builder)
        {
            builder.HasBaseType<Base>();

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.Property(x => x.DerivedField1)
                .IsRequired();

            builder.Property(x => x.AssociatedId)
                .IsRequired();

            builder.HasOne(x => x.Associated)
                .WithMany()
                .HasForeignKey(x => x.AssociatedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Composites)
                .WithOne()
                .HasForeignKey(x => x.DerivedId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}