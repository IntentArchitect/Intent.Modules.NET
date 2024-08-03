using EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_MiddleAbstract_LeafConfiguration : IEntityTypeConfiguration<TPH_MiddleAbstract_Leaf>
    {
        public void Configure(EntityTypeBuilder<TPH_MiddleAbstract_Leaf> builder)
        {
            builder.Property(x => x.MiddleAttribute)
                .IsRequired();

            builder.Property(x => x.LeafAttribute)
                .IsRequired();
        }
    }
}