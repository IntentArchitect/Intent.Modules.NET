using GraphQL.CQRS.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Infrastructure.Persistence.Configurations
{
    public class ProfitCenterConfiguration : IEntityTypeConfiguration<ProfitCenter>
    {
        public void Configure(EntityTypeBuilder<ProfitCenter> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}