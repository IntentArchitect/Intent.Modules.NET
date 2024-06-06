using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.Enums
{
    public class ClassWithEnumsConfiguration : IEntityTypeConfiguration<ClassWithEnums>
    {
        public void Configure(EntityTypeBuilder<ClassWithEnums> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EnumWithDefaultLiteral)
                .IsRequired();

            builder.Property(x => x.EnumWithoutDefaultLiteral)
                .IsRequired();

            builder.Property(x => x.EnumWithoutValues)
                .IsRequired();

            builder.Property(x => x.NullibleEnumWithDefaultLiteral);

            builder.Property(x => x.NullibleEnumWithoutDefaultLiteral);

            builder.Property(x => x.NullibleEnumWithoutValues);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}