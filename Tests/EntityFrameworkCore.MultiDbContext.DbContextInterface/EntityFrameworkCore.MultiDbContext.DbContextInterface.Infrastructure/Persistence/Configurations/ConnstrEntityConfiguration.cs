using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Persistence.Configurations
{
    public class ConnstrEntityConfiguration : IEntityTypeConfiguration<ConnstrEntity>
    {
        public void Configure(EntityTypeBuilder<ConnstrEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Message)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}