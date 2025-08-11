using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.EdgeCompositeHandling
{
    public class Level2Configuration : IEntityTypeConfiguration<Level2>
    {
        public void Configure(EntityTypeBuilder<Level2> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Level1Id)
                .IsRequired();
        }
    }
}