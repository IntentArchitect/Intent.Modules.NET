using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.SoftDelete;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.SoftDelete
{
    public class ChildOfTableParentConfiguration : IEntityTypeConfiguration<ChildOfTableParent>
    {
        public void Configure(EntityTypeBuilder<ChildOfTableParent> builder)
        {
            builder.HasBaseType<AbstractParentWithTable>();
        }
    }
}