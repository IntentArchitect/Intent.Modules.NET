using EntityFrameworkCore.SqlServer.EF7.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations
{
    public class StringKeyDefaultConfiguration : IEntityTypeConfiguration<StringKeyDefault>
    {
        public void Configure(EntityTypeBuilder<StringKeyDefault> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}