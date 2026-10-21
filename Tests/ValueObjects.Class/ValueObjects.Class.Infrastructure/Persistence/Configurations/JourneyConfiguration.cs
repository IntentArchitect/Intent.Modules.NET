using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects.Class.Domain;
using ValueObjects.Class.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace ValueObjects.Class.Infrastructure.Persistence.Configurations
{
    public class JourneyConfiguration : IEntityTypeConfiguration<Journey>
    {
        public void Configure(EntityTypeBuilder<Journey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                    .IsRequired();

            builder.OwnsOne(x => x.Route, ConfigureRoute)
                .Navigation(x => x.Route).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureRoute(OwnedNavigationBuilder<Journey, Route> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Name)
                    .IsRequired();

            builder.OwnsOne(x => x.OneToOne, ConfigureOneToOne)
                .Navigation(x => x.OneToOne).IsRequired();

            builder.OwnsOne(x => x.OneToZeroOrOne, ConfigureOneToZeroOrOne);

            builder.OwnsMany(x => x.OneToMany, ConfigureOneToMany);

            builder.OwnsOne(x => x.ManyToOne, ConfigureManyToOne)
                .Navigation(x => x.ManyToOne).IsRequired();

            builder.OwnsOne(x => x.ManyToZeroOrOne, ConfigureManyToZeroOrOne);

            builder.OwnsMany(x => x.ManyToMany, ConfigureManyToMany);

            builder.OwnsOne(x => x.ZeroOrOneToOne, ConfigureZeroOrOneToOne)
                .Navigation(x => x.ZeroOrOneToOne).IsRequired();

            builder.OwnsMany(x => x.ZeroOrOneToMany, ConfigureZeroOrOneToMany);

            builder.OwnsOne(x => x.ZeroOrOneToZeroOrOne, ConfigureZeroOrOneToZeroOrOne);
        }

        public static void ConfigureOneToOne(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureOneToZeroOrOne(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureOneToMany(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureManyToOne(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureManyToZeroOrOne(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureManyToMany(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureZeroOrOneToOne(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureZeroOrOneToMany(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }

        public static void ConfigureZeroOrOneToZeroOrOne(OwnedNavigationBuilder<Route, Waypoint> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Latitude)
                    .IsRequired();

            builder.Property(x => x.Longitude)
                    .IsRequired();
        }
    }
}