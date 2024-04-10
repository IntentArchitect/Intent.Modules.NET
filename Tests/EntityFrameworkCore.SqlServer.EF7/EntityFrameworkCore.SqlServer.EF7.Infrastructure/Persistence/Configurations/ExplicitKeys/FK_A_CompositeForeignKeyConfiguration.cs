using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class FK_A_CompositeForeignKeyConfiguration : IEntityTypeConfiguration<FK_A_CompositeForeignKey>
    {
        public void Configure(EntityTypeBuilder<FK_A_CompositeForeignKey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PK_A_CompositeKeyCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.PK_A_CompositeKeyCompositeKeyB)
                .IsRequired();

            builder.HasOne(x => x.PK_A_CompositeKey)
                .WithMany()
                .HasForeignKey(x => new { x.PK_A_CompositeKeyCompositeKeyA, x.PK_A_CompositeKeyCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}