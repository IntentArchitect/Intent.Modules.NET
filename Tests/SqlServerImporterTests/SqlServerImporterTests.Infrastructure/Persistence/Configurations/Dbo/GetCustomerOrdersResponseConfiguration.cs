using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class GetCustomerOrdersResponseConfiguration : IEntityTypeConfiguration<GetCustomerOrdersResponse>
    {
        public void Configure(EntityTypeBuilder<GetCustomerOrdersResponse> builder)
        {
            builder.HasNoKey().ToView(null);
        }
    }
}