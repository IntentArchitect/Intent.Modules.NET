using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations.NestedAssociations
{
    public class TextureConfiguration : IEntityTypeConfiguration<Texture>
    {
        public void Configure(EntityTypeBuilder<Texture> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.TextureAttribute)
                .IsRequired();
        }
    }
}