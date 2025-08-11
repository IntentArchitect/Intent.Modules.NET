using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.EdgeCompositeHandling
{
    public class ExternalConfiguration : IEntityTypeConfiguration<External>
    {
        public void Configure(EntityTypeBuilder<External> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Level2Id)
                .IsRequired();

            builder.HasOne(x => x.Level2)
                .WithMany()
                .HasForeignKey(x => x.Level2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}