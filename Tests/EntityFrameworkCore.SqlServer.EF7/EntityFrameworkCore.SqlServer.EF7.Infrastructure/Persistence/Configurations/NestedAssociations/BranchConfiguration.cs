using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.NestedAssociations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BranchAttribute)
                .IsRequired();

            builder.Property(x => x.TextureId)
                .IsRequired();

            builder.Property(x => x.TreeId)
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

            builder.HasMany(x => x.Leaves)
                .WithOne()
                .HasForeignKey(x => x.BranchId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        public void ConfigureInternode(OwnedNavigationBuilder<Branch, Internode> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InternodeAttribute)
                .IsRequired();
        }
    }
}