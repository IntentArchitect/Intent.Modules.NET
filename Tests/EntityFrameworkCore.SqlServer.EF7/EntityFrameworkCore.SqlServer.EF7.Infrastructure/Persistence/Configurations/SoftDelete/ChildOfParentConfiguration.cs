using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.SoftDelete;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.SoftDelete
{
    public class ChildOfParentConfiguration : IEntityTypeConfiguration<ChildOfParent>
    {
        public void Configure(EntityTypeBuilder<ChildOfParent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.HasQueryFilter(t => t.IsDeleted == false);
        }
    }
}