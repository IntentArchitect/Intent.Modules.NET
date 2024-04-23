using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class TPC_FkDerivedClassConfiguration : IEntityTypeConfiguration<TPC_FkDerivedClass>
    {
        public void Configure(EntityTypeBuilder<TPC_FkDerivedClass> builder)
        {
            builder.ToTable("TpcFkDerivedClass");

            builder.Property(x => x.DerivedField)
                .IsRequired();
        }
    }
}