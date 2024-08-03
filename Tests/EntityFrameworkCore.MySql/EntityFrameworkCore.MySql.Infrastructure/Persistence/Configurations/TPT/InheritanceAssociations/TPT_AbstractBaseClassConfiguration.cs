using EntityFrameworkCore.MySql.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_AbstractBaseClassConfiguration : IEntityTypeConfiguration<TPT_AbstractBaseClass>
    {
        public void Configure(EntityTypeBuilder<TPT_AbstractBaseClass> builder)
        {
            builder.ToTable("TptAbstractBaseClass");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}