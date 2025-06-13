using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Persistence.Configurations
{
    public class ProductInMemoryConfiguration : IEntityTypeConfiguration<ProductInMemory>
    {
        public void Configure(EntityTypeBuilder<ProductInMemory> builder)
        {
            builder.HasNoKey().ToView(null);
        }
    }
}