using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations
{
    public class StakeholderConfiguration : IEntityTypeConfiguration<Stakeholder>
    {
        public void Configure(EntityTypeBuilder<Stakeholder> builder)
        {
            builder.ToTable("Stakeholders", "stakeholder");

            builder.HasKey(x => x.StakeholderId);

            builder.Property(x => x.StakeholderId)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.DateCreated)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}