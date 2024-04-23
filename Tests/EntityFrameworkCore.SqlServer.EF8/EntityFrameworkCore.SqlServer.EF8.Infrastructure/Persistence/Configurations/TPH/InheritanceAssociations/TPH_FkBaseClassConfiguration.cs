using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_FkBaseClassConfiguration : IEntityTypeConfiguration<TPH_FkBaseClass>
    {
        public void Configure(EntityTypeBuilder<TPH_FkBaseClass> builder)
        {
            builder.HasKey(x => new { x.CompositeKeyA, x.CompositeKeyB });
        }
    }
}