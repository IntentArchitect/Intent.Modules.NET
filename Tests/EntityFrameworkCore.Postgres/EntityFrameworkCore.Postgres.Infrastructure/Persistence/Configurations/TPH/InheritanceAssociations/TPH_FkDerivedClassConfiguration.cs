using EntityFrameworkCore.Postgres.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_FkDerivedClassConfiguration : IEntityTypeConfiguration<TPH_FkDerivedClass>
    {
        public void Configure(EntityTypeBuilder<TPH_FkDerivedClass> builder)
        {
            builder.HasBaseType<TPH_FkBaseClass>();

            builder.Property(x => x.DerivedField)
                .IsRequired();
        }
    }
}