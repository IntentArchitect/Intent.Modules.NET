using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure.Persistence.Configurations
{
    public class ClassAConfiguration : IEntityTypeConfiguration<ClassA>
    {
        public void Configure(EntityTypeBuilder<ClassA> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.OwnsMany(x => x.ClassBS, ConfigureClassBS);
        }

        public void ConfigureClassBS(OwnedNavigationBuilder<ClassA, ClassB> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ClassAId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClassAId)
                .IsRequired();

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}