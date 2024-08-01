using EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForAbstractConfiguration : IEntityTypeConfiguration<TPH_DerivedClassForAbstract>
    {
        public void Configure(EntityTypeBuilder<TPH_DerivedClassForAbstract> builder)
        {
            builder.HasBaseType<TPH_AbstractBaseClass>();

            builder.Property(x => x.DerivedAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}