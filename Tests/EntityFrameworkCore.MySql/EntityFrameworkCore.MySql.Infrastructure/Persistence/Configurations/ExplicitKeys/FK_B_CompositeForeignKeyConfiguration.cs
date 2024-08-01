using EntityFrameworkCore.MySql.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class FK_B_CompositeForeignKeyConfiguration : IEntityTypeConfiguration<FK_B_CompositeForeignKey>
    {
        public void Configure(EntityTypeBuilder<FK_B_CompositeForeignKey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PK_CompositeKeyCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.PK_CompositeKeyCompositeKeyB)
                .IsRequired();

            builder.HasOne(x => x.PK_CompositeKey)
                .WithMany()
                .HasForeignKey(x => new { x.PK_CompositeKeyCompositeKeyA, x.PK_CompositeKeyCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}