using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForConcreteConfiguration : IEntityTypeConfiguration<TPT_DerivedClassForConcrete>
    {
        public void Configure(EntityTypeBuilder<TPT_DerivedClassForConcrete> builder)
        {
            builder.ToTable("TptDerivedClassForConcrete");

            builder.Property(x => x.DerivedAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}