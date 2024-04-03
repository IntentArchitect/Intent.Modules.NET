using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(100)");

            builder.Property(x => x.Surname)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.PreferencesSpecials)
                .HasColumnName("Preferences_Specials");

            builder.Property(x => x.PreferencesNewsletter)
                .HasColumnName("Preferences_Newsletter");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}