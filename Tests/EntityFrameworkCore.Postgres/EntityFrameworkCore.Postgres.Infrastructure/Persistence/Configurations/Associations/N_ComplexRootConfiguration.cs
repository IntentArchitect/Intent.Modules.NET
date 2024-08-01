using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations
{
    public class N_ComplexRootConfiguration : IEntityTypeConfiguration<N_ComplexRoot>
    {
        public void Configure(EntityTypeBuilder<N_ComplexRoot> builder)
        {
            builder.HasKey(x => x.Id);

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
        }

        public void ConfigureN_CompositeManies(OwnedNavigationBuilder<N_ComplexRoot, N_CompositeMany> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.NComplexrootId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ManyAttr)
                .IsRequired();

            builder.Property(x => x.NComplexrootId)
                .IsRequired();
        }
    }
}