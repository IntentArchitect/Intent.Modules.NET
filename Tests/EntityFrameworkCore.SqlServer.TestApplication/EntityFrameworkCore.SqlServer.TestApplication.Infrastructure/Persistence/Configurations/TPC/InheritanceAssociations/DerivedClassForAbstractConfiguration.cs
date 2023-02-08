using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class DerivedClassForAbstractConfiguration : IEntityTypeConfiguration<DerivedClassForAbstract>
    {
        public void Configure(EntityTypeBuilder<DerivedClassForAbstract> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttribute)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.DerivedAttribute)
                .IsRequired()
                .HasMaxLength(250);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}