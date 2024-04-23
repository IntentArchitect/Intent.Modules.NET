using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.Associations
{
    public class M_SelfReferenceBiNavConfiguration : IEntityTypeConfiguration<M_SelfReferenceBiNav>
    {
        public void Configure(EntityTypeBuilder<M_SelfReferenceBiNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SelfRefBiNavAttr)
                .IsRequired();

            builder.Property(x => x.M_SelfReferenceBiNavDstId);

            builder.HasOne(x => x.M_SelfReferenceBiNavDst)
                .WithMany(x => x.M_SelfReferenceBiNavs)
                .HasForeignKey(x => x.M_SelfReferenceBiNavDstId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}