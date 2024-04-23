using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class ParentNonStdIdConfiguration : IEntityTypeConfiguration<ParentNonStdId>
    {
        public void Configure(EntityTypeBuilder<ParentNonStdId> builder)
        {
            builder.HasKey(x => x.MyId);

            builder.Property(x => x.Desc)
                .IsRequired();

            builder.HasOne(x => x.ChildNonStdId)
                .WithOne()
                .HasForeignKey<ChildNonStdId>(x => x.DiffId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}