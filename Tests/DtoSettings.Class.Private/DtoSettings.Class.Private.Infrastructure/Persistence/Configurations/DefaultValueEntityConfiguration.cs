using DtoSettings.Class.Private.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace DtoSettings.Class.Private.Infrastructure.Persistence.Configurations
{
    public class DefaultValueEntityConfiguration : IEntityTypeConfiguration<DefaultValueEntity>
    {
        public void Configure(EntityTypeBuilder<DefaultValueEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StringDefault)
                .IsRequired();

            builder.Property(x => x.IntDefault)
                .IsRequired();

            builder.Property(x => x.EnumDefault)
                .IsRequired();
        }
    }
}