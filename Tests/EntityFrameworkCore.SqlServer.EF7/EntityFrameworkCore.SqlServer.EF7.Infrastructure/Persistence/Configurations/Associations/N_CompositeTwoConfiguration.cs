using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.Associations
{
    public class N_CompositeTwoConfiguration : IEntityTypeConfiguration<N_CompositeTwo>
    {
        public void Configure(EntityTypeBuilder<N_CompositeTwo> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeTwoAttr)
                .IsRequired();
        }
    }
}