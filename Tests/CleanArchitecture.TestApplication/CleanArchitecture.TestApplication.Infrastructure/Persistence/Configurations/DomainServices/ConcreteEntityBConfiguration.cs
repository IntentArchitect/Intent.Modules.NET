using CleanArchitecture.TestApplication.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.DomainServices
{
    public class ConcreteEntityBConfiguration : IEntityTypeConfiguration<ConcreteEntityB>
    {
        public void Configure(EntityTypeBuilder<ConcreteEntityB> builder)
        {
            builder.HasBaseType<BaseEntityB>();

            builder.Property(x => x.ConcreteAttr)
                .IsRequired();
        }
    }
}