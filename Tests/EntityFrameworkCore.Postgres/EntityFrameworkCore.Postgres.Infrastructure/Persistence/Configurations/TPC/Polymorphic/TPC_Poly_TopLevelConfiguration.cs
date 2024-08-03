using EntityFrameworkCore.Postgres.Domain.Entities.TPC.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPC.Polymorphic
{
    public class TPC_Poly_TopLevelConfiguration : IEntityTypeConfiguration<TPC_Poly_TopLevel>
    {
        public void Configure(EntityTypeBuilder<TPC_Poly_TopLevel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TopField)
                .IsRequired();
        }
    }
}