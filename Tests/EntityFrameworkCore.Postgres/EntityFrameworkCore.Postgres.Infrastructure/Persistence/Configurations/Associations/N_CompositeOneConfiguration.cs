using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations
{
    public class N_CompositeOneConfiguration : IEntityTypeConfiguration<N_CompositeOne>
    {
        public void Configure(EntityTypeBuilder<N_CompositeOne> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeOneAttr)
                .IsRequired();
        }
    }
}