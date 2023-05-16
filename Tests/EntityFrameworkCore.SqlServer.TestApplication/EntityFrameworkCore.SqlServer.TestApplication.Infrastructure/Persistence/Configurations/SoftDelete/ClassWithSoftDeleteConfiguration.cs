using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.SoftDelete;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.SoftDelete
{
    public class ClassWithSoftDeleteConfiguration : IEntityTypeConfiguration<ClassWithSoftDelete>
    {
        public void Configure(EntityTypeBuilder<ClassWithSoftDelete> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute1)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);

            builder.HasQueryFilter(t => t.IsDeleted == false);
        }
    }
}