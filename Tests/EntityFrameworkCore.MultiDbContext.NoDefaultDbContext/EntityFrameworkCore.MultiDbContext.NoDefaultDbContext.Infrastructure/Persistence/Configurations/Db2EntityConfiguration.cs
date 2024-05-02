using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Persistence.Configurations
{
    public class Db2EntityConfiguration : IEntityTypeConfiguration<Db2Entity>
    {
        public void Configure(EntityTypeBuilder<Db2Entity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Message)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}