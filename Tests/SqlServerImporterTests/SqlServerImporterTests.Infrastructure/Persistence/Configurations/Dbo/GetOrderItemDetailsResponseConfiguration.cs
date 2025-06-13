using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class GetOrderItemDetailsResponseConfiguration : IEntityTypeConfiguration<GetOrderItemDetailsResponse>
    {
        public void Configure(EntityTypeBuilder<GetOrderItemDetailsResponse> builder)
        {
            builder.HasNoKey().ToView(null);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
        }
    }
}