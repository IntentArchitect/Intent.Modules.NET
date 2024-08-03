using EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_AbstractBaseClassConfiguration : IEntityTypeConfiguration<TPH_AbstractBaseClass>
    {
        public void Configure(EntityTypeBuilder<TPH_AbstractBaseClass> builder)
        {
            builder.ToTable("TphAbstractBaseClass");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}