using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class FuneralCoverQuoteConfiguration : IEntityTypeConfiguration<FuneralCoverQuote>
    {
        public void Configure(EntityTypeBuilder<FuneralCoverQuote> builder)
        {
            builder.HasBaseType<Quote>();

            builder.Property(x => x.Amount)
                .IsRequired();
        }
    }
}