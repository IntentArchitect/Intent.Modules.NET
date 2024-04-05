using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.NestedAssociations
{
    public class LeafConfiguration : IEntityTypeConfiguration<Leaf>
    {
        public void Configure(EntityTypeBuilder<Leaf> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.LeafAttribute)
                .IsRequired();

            builder.Property(x => x.SunId)
                .IsRequired();

            builder.Property(x => x.BranchId)
                .IsRequired();

            builder.HasMany(x => x.Worms)
                .WithOne()
                .HasForeignKey(x => x.LeafId);

            builder.HasOne(x => x.Sun)
                .WithMany()
                .HasForeignKey(x => x.SunId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}