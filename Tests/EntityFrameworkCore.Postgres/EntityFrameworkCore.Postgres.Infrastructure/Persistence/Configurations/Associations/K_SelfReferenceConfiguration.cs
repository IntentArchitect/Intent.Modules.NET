using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations
{
    public class K_SelfReferenceConfiguration : IEntityTypeConfiguration<K_SelfReference>
    {
        public void Configure(EntityTypeBuilder<K_SelfReference> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SelfRefAttr)
                .IsRequired();

            builder.Property(x => x.K_SelfReferenceAssociationId);

            builder.HasOne(x => x.K_SelfReferenceAssociation)
                .WithMany()
                .HasForeignKey(x => x.K_SelfReferenceAssociationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}