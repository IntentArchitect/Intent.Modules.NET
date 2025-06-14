using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations.MappableStoredProcs
{
    public class EntityRecordConfiguration : IEntityTypeConfiguration<EntityRecord>
    {
        public void Configure(EntityTypeBuilder<EntityRecord> builder)
        {
            builder.HasNoKey().ToView(null);
        }
    }
}