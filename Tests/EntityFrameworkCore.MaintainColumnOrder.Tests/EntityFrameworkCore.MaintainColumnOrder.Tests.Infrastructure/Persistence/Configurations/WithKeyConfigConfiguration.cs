using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Persistence.Configurations
{
    public class WithKeyConfigConfiguration : IEntityTypeConfiguration<WithKeyConfig>
    {
        public void Configure(EntityTypeBuilder<WithKeyConfig> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("BobId")
                .HasColumnOrder(0);

            builder.Property(x => x.Col1)
                .IsRequired()
                .HasColumnOrder(1);

            builder.Property(x => x.Col2)
                .IsRequired()
                .HasColumnOrder(2);

            builder.Property(x => x.Col3)
                .IsRequired()
                .HasColumnOrder(3);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}