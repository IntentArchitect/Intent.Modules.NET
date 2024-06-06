using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.Inheritance
{
    public class ConcreteClassConfiguration : IEntityTypeConfiguration<ConcreteClass>
    {
        public void Configure(EntityTypeBuilder<ConcreteClass> builder)
        {
            builder.HasBaseType<BaseClass>();

            builder.Property(x => x.ConcreteAttr)
                .IsRequired();
        }
    }
}