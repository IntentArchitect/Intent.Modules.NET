using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Persistence.Configurations
{
    public class CustomerSegmentsConfiguration : IEntityTypeConfiguration<CustomerSegments>
    {
        public void Configure(EntityTypeBuilder<CustomerSegments> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SegmentId)
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.ClassificationSource)
                .IsRequired();

            builder.Property(x => x.Confidence)
                .IsRequired();

            builder.HasOne(x => x.Segment)
                .WithMany()
                .HasForeignKey(x => x.SegmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerSegments)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}