using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_FkBaseClassConfiguration : IEntityTypeConfiguration<TPT_FkBaseClass>
    {
        public void Configure(EntityTypeBuilder<TPT_FkBaseClass> builder)
        {
            builder.ToTable("TptFkBaseClass");

            builder.HasKey(x => new { x.CompositeKeyA, x.CompositeKeyB });
        }
    }
}