using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForAbstractConfiguration : IEntityTypeConfiguration<TPT_DerivedClassForAbstract>
    {
        public void Configure(EntityTypeBuilder<TPT_DerivedClassForAbstract> builder)
        {
            builder.ToTable("TptDerivedClassForAbstract");

            builder.Property(x => x.DerivedAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}