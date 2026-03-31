using DtoSettings.Class.Protected.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace DtoSettings.Class.Protected.Infrastructure.Persistence.Configurations
{
    public class CollectionEntityConfiguration : IEntityTypeConfiguration<CollectionEntity>
    {
        public void Configure(EntityTypeBuilder<CollectionEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Collection)
                .IsRequired();
        }
    }
}