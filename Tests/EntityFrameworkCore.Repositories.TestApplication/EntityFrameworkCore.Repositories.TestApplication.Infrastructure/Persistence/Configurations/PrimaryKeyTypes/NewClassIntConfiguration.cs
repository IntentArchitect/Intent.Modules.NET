using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations.PrimaryKeyTypes
{
    public class NewClassIntConfiguration : IEntityTypeConfiguration<NewClassInt>
    {
        public void Configure(EntityTypeBuilder<NewClassInt> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IntName)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}