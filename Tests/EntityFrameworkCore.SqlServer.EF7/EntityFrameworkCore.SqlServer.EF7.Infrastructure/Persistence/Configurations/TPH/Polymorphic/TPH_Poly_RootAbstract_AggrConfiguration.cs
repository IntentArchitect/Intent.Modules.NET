using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.TPH.Polymorphic
{
    public class TPH_Poly_RootAbstract_AggrConfiguration : IEntityTypeConfiguration<TPH_Poly_RootAbstract_Aggr>
    {
        public void Configure(EntityTypeBuilder<TPH_Poly_RootAbstract_Aggr> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggrField)
                .IsRequired();
        }
    }
}