using EntityFrameworkCore.Postgres.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_MiddleAbstract_RootConfiguration : IEntityTypeConfiguration<TPH_MiddleAbstract_Root>
    {
        public void Configure(EntityTypeBuilder<TPH_MiddleAbstract_Root> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RootAttribute)
                .IsRequired();
        }
    }
}