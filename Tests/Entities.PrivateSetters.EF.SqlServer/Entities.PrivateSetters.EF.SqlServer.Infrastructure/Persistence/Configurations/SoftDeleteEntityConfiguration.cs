using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Infrastructure.Persistence.Configurations
{
    public class SoftDeleteEntityConfiguration : IEntityTypeConfiguration<SoftDeleteEntity>
    {
        public void Configure(EntityTypeBuilder<SoftDeleteEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.HasQueryFilter(t => t.IsDeleted == false);
        }
    }
}