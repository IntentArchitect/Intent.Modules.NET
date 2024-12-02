using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Infrastructure.Persistence.Configurations
{
    public class CustomConstructorConfiguration : IEntityTypeConfiguration<CustomConstructor>
    {
        public void Configure(EntityTypeBuilder<CustomConstructor> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Col1)
                .IsRequired();

            builder.Property(x => x.Col2)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}