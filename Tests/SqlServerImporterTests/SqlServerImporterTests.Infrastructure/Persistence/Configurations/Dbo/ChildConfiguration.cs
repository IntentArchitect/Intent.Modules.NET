using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class ChildConfiguration : IEntityTypeConfiguration<Child>
    {
        public void Configure(EntityTypeBuilder<Child> builder)
        {
            builder.ToTable("Children", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ParentId)
                .IsRequired();

            builder.Property(x => x.ParentId2)
                .IsRequired();

            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => new { x.ParentId, x.ParentId2 })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}