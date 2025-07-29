using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRoot5EntityWithRepoConfiguration : IEntityTypeConfiguration<AggregateRoot5EntityWithRepo>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot5EntityWithRepo> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}