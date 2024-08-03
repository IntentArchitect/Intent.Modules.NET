using EntityFrameworkCore.MySql.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.Indexes
{
    public class WithBaseIndexBaseConfiguration : IEntityTypeConfiguration<WithBaseIndexBase>
    {
        public void Configure(EntityTypeBuilder<WithBaseIndexBase> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Created)
                .IsRequired();
        }
    }
}