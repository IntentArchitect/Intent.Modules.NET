using EntityFrameworkCore.MySql.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations
{
    public class SchemaParentConfiguration : IEntityTypeConfiguration<SchemaParent>
    {
        public void Configure(EntityTypeBuilder<SchemaParent> builder)
        {
            builder.ToTable("SchemaParents", "myapp");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsOne(x => x.SchemaInLineChild, ConfigureSchemaInLineChild)
                .Navigation(x => x.SchemaInLineChild).IsRequired();
        }

        public void ConfigureSchemaInLineChild(OwnedNavigationBuilder<SchemaParent, SchemaInLineChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}