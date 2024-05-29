using CleanArchitecture.Comprehensive.Domain.Entities.OperationAndConstructorMapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.OperationAndConstructorMapping
{
    public class OpAndCtorMapping2Configuration : IEntityTypeConfiguration<OpAndCtorMapping2>
    {
        public void Configure(EntityTypeBuilder<OpAndCtorMapping2> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.OpAndCtorMapping1, ConfigureOpAndCtorMapping1)
                .Navigation(x => x.OpAndCtorMapping1).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureOpAndCtorMapping1(OwnedNavigationBuilder<OpAndCtorMapping2, OpAndCtorMapping1> builder)
        {
            builder.WithOwner(x => x.OpAndCtorMapping2)
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OpAndCtorMapping3Id)
                .IsRequired();

            builder.HasOne(x => x.OpAndCtorMapping3)
                .WithMany()
                .HasForeignKey(x => x.OpAndCtorMapping3Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}