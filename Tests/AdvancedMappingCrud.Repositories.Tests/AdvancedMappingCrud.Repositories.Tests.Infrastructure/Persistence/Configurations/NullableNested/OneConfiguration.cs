using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.NullableNested
{
    public class OneConfiguration : IEntityTypeConfiguration<One>
    {
        public void Configure(EntityTypeBuilder<One> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OneName)
                .IsRequired();

            builder.Property(x => x.OneAge)
                .IsRequired();

            builder.OwnsOne(x => x.Two, ConfigureTwo)
                .Navigation(x => x.Two).IsRequired();

            builder.OwnsOne(x => x.Four, ConfigureFour);

            builder.OwnsMany(x => x.Fives, ConfigureFives);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureTwo(OwnedNavigationBuilder<One, Two> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TwoName)
                .IsRequired();

            builder.Property(x => x.TwoAge)
                .IsRequired();
        }

        public static void ConfigureFour(OwnedNavigationBuilder<One, Four> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FourName)
                .IsRequired();

            builder.Property(x => x.FourAge)
                .IsRequired();

            builder.OwnsOne(x => x.Three, ConfigureThree);
        }

        public static void ConfigureThree(OwnedNavigationBuilder<Four, Three> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ThreeName)
                .IsRequired();

            builder.Property(x => x.ThreeAge)
                .IsRequired();
        }

        public static void ConfigureFives(OwnedNavigationBuilder<One, Five> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OneId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FiveName)
                .IsRequired();

            builder.Property(x => x.OneId)
                .IsRequired();

            builder.Property(x => x.FiveAge)
                .IsRequired();
        }
    }
}