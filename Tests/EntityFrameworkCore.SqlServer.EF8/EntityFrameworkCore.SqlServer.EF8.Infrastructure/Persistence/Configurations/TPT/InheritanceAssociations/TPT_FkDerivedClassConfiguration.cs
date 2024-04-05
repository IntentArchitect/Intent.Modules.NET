using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_FkDerivedClassConfiguration : IEntityTypeConfiguration<TPT_FkDerivedClass>
    {
        public void Configure(EntityTypeBuilder<TPT_FkDerivedClass> builder)
        {
            builder.ToTable("TptFkDerivedClass");

            builder.Property(x => x.DerivedField)
                .IsRequired();
        }
    }
}