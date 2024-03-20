using CleanArchitecture.TestApplication.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.DomainServices
{
    public class ConcreteEntityAConfiguration : IEntityTypeConfiguration<ConcreteEntityA>
    {
        public void Configure(EntityTypeBuilder<ConcreteEntityA> builder)
        {
            builder.HasBaseType<BaseEntityA>();

            builder.Property(x => x.ConcreteAttr)
                .IsRequired();
        }
    }
}