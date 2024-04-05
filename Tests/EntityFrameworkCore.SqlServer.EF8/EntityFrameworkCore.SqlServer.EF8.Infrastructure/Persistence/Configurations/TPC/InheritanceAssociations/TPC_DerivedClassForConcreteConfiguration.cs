using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForConcreteConfiguration : IEntityTypeConfiguration<TPC_DerivedClassForConcrete>
    {
        public void Configure(EntityTypeBuilder<TPC_DerivedClassForConcrete> builder)
        {
            builder.ToTable("TpcDerivedClassForConcrete");

            builder.Property(x => x.DerivedAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}