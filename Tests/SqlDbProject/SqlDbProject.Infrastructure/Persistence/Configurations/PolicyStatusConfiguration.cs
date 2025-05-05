using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations
{
    public class PolicyStatusConfiguration : IEntityTypeConfiguration<PolicyStatus>
    {
        public void Configure(EntityTypeBuilder<PolicyStatus> builder)
        {
            builder.ToTable("PolicyStatuses", "accountholder");

            builder.HasKey(x => x.PolicyStatusId);

            builder.Property(x => x.PolicyStatusId)
                .ValueGeneratedNever();

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}