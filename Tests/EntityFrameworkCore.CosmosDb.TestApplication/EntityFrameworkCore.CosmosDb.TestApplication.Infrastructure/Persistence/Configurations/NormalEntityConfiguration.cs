using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations
{
    public class NormalEntityConfiguration : IEntityTypeConfiguration<NormalEntity>
    {
        public void Configure(EntityTypeBuilder<NormalEntity> builder)
        {
            builder.ToContainer("PackageContainer");

            builder.HasPartitionKey(x => x.PackagePartKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PackagePartKey)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}