using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.NestedAssociations
{
    public class WormConfiguration : IEntityTypeConfiguration<Worm>
    {
        public void Configure(EntityTypeBuilder<Worm> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Color)
                .IsRequired();

            builder.Property(x => x.LeafId);
        }
    }
}