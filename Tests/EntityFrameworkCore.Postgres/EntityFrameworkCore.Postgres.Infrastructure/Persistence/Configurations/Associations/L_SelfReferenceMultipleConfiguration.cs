using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations
{
    public class L_SelfReferenceMultipleConfiguration : IEntityTypeConfiguration<L_SelfReferenceMultiple>
    {
        public void Configure(EntityTypeBuilder<L_SelfReferenceMultiple> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SelfRefMultipleAttr)
                .IsRequired();

            builder.HasMany(x => x.L_SelfReferenceMultiplesDst)
                .WithMany("L_SelfReferenceMultiplesSrc")
                .UsingEntity(x => x.ToTable("L_SelfReferenceMultipleL_SelfReferenceMultiples"));
        }
    }
}