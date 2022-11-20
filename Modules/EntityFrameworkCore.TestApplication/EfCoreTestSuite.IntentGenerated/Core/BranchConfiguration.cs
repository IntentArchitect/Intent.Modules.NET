using EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BranchAttribute)
                .IsRequired();

            builder.HasOne(x => x.Texture)
                .WithMany()
                .HasForeignKey(x => x.TextureId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Internode, ConfigureInternode)
                .Navigation(x => x.Internode).IsRequired();

            builder.HasMany(x => x.Inhabitants)
                .WithMany("Branches")
                .UsingEntity(x => x.ToTable("BranchInhabitants"));

            builder.OwnsMany(x => x.Leaves, ConfigureLeaves);
        }

        public void ConfigureInternode(OwnedNavigationBuilder<Branch, Internode> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InternodeAttribute)
                .IsRequired();
        }

        public void ConfigureLeaves(OwnedNavigationBuilder<Branch, Leaf> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.BranchId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LeafAttribute)
                .IsRequired();
        }
    }
}