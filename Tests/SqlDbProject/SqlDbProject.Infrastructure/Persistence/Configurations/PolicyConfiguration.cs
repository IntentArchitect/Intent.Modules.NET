using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations
{
    public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.ToTable("Policies", "policy");

            builder.HasKey(x => x.PolicyId);

            builder.Property(x => x.PolicyId)
                .ValueGeneratedNever();

            builder.Property(x => x.PolicyStatusId)
                .IsRequired();

            builder.Property(x => x.StakeholderId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.PolicyNumber)
                .IsRequired();

            builder.Property(x => x.OriginalInceptionDate)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.ReviewDate);

            builder.Property(x => x.ExpiryDate);

            builder.Property(x => x.ExternalSystemReference);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasIndex(x => x.PolicyId)
                .HasDatabaseName("IX_PolicyItem_PolicyId");

            builder.HasIndex(x => new { x.PolicyStatusId, x.StakeholderId })
                .HasDatabaseName("IX_Policy_Stakeholder");

            builder.HasOne(x => x.PolicyStatus)
                .WithMany()
                .HasForeignKey(x => x.PolicyStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Stakeholder)
                .WithMany()
                .HasForeignKey(x => x.StakeholderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}