using EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class TreeConfiguration : IEntityTypeConfiguration<Tree>
    {
        public void Configure(EntityTypeBuilder<Tree> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TreeAttribute)
                .IsRequired();

            builder.OwnsMany(x => x.Branches, ConfigureBranches);
        }

        public void ConfigureBranches(OwnedNavigationBuilder<Tree, Branch> builder)
        {
            builder.WithOwner(x => x.Tree)
                .HasForeignKey(x => x.TreeId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BranchAttribute)
                .IsRequired();

            builder.HasOne(x => x.Texture)
                .WithMany()
                .HasForeignKey(x => x.TextureId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Internode)
                .WithOne()
                .HasForeignKey<Branch>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // builder.HasMany(x => x.Inhabitants)
            //     .WithMany("Branches")
            //     .UsingEntity(x => x.ToTable("BranchInhabitants"));

            builder.OwnsMany(x => x.Leaves, ConfigureLeaves);
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