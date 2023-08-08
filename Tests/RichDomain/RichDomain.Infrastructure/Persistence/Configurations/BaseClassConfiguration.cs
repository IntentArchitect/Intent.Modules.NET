using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RichDomain.Infrastructure.Persistence.Configurations
{
    public class BaseClassConfiguration : IEntityTypeConfiguration<BaseClass>
    {
        public void Configure(EntityTypeBuilder<BaseClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttribute)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}