using CleanArchitecture.TestApplication.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.DomainServices
{
    public class BaseEntityAConfiguration : IEntityTypeConfiguration<BaseEntityA>
    {
        public void Configure(EntityTypeBuilder<BaseEntityA> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttr)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}