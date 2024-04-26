using EntityFrameworkCore.MultiConnectionStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MultiConnectionStrings.Infrastructure.Persistence.Configurations
{
    public class ClassBConfiguration : IEntityTypeConfiguration<ClassB>
    {
        public void Configure(EntityTypeBuilder<ClassB> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Message)
                .IsRequired();
        }
    }
}