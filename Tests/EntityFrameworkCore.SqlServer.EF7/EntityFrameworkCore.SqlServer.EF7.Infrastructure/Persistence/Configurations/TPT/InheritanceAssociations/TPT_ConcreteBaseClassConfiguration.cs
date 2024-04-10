using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_ConcreteBaseClassConfiguration : IEntityTypeConfiguration<TPT_ConcreteBaseClass>
    {
        public void Configure(EntityTypeBuilder<TPT_ConcreteBaseClass> builder)
        {
            builder.ToTable("TptConcreteBaseClass");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseAttribute)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}