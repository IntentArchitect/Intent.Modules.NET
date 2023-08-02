using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations
{
    public class ChildNonStdIdConfiguration : IEntityTypeConfiguration<ChildNonStdId>
    {
        public void Configure(EntityTypeBuilder<ChildNonStdId> builder)
        {
            builder.HasKey(x => x.DiffId);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}