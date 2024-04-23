using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForConcreteConfiguration : IEntityTypeConfiguration<TPH_DerivedClassForConcrete>
    {
        public void Configure(EntityTypeBuilder<TPH_DerivedClassForConcrete> builder)
        {
            builder.HasBaseType<TPH_ConcreteBaseClass>();

            builder.Property(x => x.DerivedAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}