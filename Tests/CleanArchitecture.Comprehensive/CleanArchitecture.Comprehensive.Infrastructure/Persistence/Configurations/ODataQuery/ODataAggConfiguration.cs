using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.ODataQuery
{
    public class ODataAggConfiguration : IEntityTypeConfiguration<ODataAgg>
    {
        public void Configure(EntityTypeBuilder<ODataAgg> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.ODataChildren, ConfigureODataChildren);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureODataChildren(OwnedNavigationBuilder<ODataAgg, ODataChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ODataAggId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.No)
                .IsRequired();

            builder.Property(x => x.ODataAggId)
                .IsRequired();
        }
    }
}