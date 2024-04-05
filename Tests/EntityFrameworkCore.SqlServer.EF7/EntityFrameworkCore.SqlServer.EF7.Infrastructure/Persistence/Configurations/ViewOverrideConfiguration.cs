using EntityFrameworkCore.SqlServer.EF7.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations
{
    public class ViewOverrideConfiguration : IEntityTypeConfiguration<ViewOverride>
    {
        public void Configure(EntityTypeBuilder<ViewOverride> builder)
        {
            builder.ToView("ViewOverrides", "myviewschema");

            builder.HasKey(x => x.Id);
        }
    }
}