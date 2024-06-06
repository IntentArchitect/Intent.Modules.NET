using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.Inheritance
{
    public class BaseClassConfiguration : IEntityTypeConfiguration<BaseClass>
    {
        public void Configure(EntityTypeBuilder<BaseClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttr)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}