using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Aggregational
{
    public class ManyToManySourceConfiguration : IEntityTypeConfiguration<ManyToManySource>
    {
        public void Configure(EntityTypeBuilder<ManyToManySource> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.HasMany(x => x.ManyToManyDests)
                .WithMany(x => x.ManyToManySources)
                .UsingEntity(x => x.ToTable("ManyToManySourceManyToManyDests"));
        }
    }
}