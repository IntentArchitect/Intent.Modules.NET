using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.NestedAssociations
{
    public class SunConfiguration : IEntityTypeConfiguration<Sun>
    {
        public void Configure(EntityTypeBuilder<Sun> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Temp)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}