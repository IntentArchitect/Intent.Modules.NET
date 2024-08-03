using EntityFrameworkCore.Postgres.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.NestedAssociations
{
    public class TreeConfiguration : IEntityTypeConfiguration<Tree>
    {
        public void Configure(EntityTypeBuilder<Tree> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TreeAttribute)
                .IsRequired();

            builder.HasMany(x => x.Branches)
                .WithOne(x => x.Tree)
                .HasForeignKey(x => x.TreeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}