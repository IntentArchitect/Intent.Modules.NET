using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class FkDerivedClassConfiguration : IEntityTypeConfiguration<FkDerivedClass>
    {
        public void Configure(EntityTypeBuilder<FkDerivedClass> builder)
        {
            builder.HasBaseType<FkBaseClass>();

            builder.Property(x => x.DerivedField)
                .IsRequired();
        }
    }
}