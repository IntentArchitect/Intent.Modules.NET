using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Persistence.Configurations
{
    public class InLineVoConfiguration : IEntityTypeConfiguration<InLineVo>
    {
        public void Configure(EntityTypeBuilder<InLineVo> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnOrder(0);

            builder.Property(x => x.Col1)
                .IsRequired()
                .HasColumnOrder(1);

            builder.OwnsOne(x => x.Col2, ConfigureCol2)
                .Navigation(x => x.Col2).IsRequired();

            builder.Property(x => x.Col3)
                .IsRequired()
                .HasColumnOrder(4);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureCol2(OwnedNavigationBuilder<InLineVo, InLineColumns> builder)
        {
            builder.Property(x => x.Col1)
                .IsRequired()
                .HasColumnOrder(2);

            builder.Property(x => x.Col2)
                .IsRequired()
                .HasColumnOrder(3);
        }
    }
}