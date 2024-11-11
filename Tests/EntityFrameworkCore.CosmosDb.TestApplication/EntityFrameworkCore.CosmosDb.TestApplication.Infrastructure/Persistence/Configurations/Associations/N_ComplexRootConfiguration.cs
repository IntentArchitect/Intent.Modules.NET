using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class N_ComplexRootConfiguration : IEntityTypeConfiguration<N_ComplexRoot>
    {
        public void Configure(EntityTypeBuilder<N_ComplexRoot> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.ComplexAttr)
                .IsRequired();

            builder.HasOne(x => x.N_CompositeOne)
                .WithOne()
                .HasForeignKey<N_CompositeOne>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.N_CompositeTwo)
                .WithOne()
                .HasForeignKey<N_CompositeTwo>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(x => x.N_CompositeManies, ConfigureN_CompositeManies);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureN_CompositeManies(OwnedNavigationBuilder<N_ComplexRoot, N_CompositeMany> builder)
        {
            builder.WithOwner();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ManyAttr)
                .IsRequired();

            builder.Property(x => x.NComplexrootId)
                .IsRequired();
        }
    }
}