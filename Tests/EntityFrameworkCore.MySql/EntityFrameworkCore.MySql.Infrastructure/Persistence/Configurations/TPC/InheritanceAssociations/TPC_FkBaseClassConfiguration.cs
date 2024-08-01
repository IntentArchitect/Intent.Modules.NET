using EntityFrameworkCore.MySql.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class TPC_FkBaseClassConfiguration : IEntityTypeConfiguration<TPC_FkBaseClass>
    {
        public void Configure(EntityTypeBuilder<TPC_FkBaseClass> builder)
        {
            builder.HasKey(x => new { x.CompositeKeyA, x.CompositeKeyB });
        }
    }
}