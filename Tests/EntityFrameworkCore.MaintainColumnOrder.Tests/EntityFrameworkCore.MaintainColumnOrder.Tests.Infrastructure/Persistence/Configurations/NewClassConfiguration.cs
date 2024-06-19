using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Persistence.Configurations
{
    public class NewClassConfiguration : IEntityTypeConfiguration<NewClass>
    {
        public void Configure(EntityTypeBuilder<NewClass> builder)
        {
            builder.HasBaseType<BaseWithLast>();

            builder.Property(x => x.Col1)
                .IsRequired()
                .HasColumnOrder(4);

            builder.Property(x => x.Col2)
                .IsRequired()
                .HasColumnOrder(5);
        }
    }
}