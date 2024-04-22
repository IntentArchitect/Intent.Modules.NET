using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations.PrimaryKeyTypes
{
    public class NewClassLongConfiguration : IEntityTypeConfiguration<NewClassLong>
    {
        public void Configure(EntityTypeBuilder<NewClassLong> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.LongName)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}