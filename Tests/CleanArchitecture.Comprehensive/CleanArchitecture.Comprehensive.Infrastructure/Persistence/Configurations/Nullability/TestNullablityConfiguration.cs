using CleanArchitecture.Comprehensive.Domain.Entities.Nullability;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.Nullability
{
    public class TestNullablityConfiguration : IEntityTypeConfiguration<TestNullablity>
    {
        public void Configure(EntityTypeBuilder<TestNullablity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SampleEnum)
                .IsRequired();

            builder.Property(x => x.Str)
                .IsRequired();

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.DateTime)
                .IsRequired();

            builder.Property(x => x.NullableGuid);

            builder.Property(x => x.NullableEnum);

            builder.Property(x => x.NullabilityPeerId)
                .IsRequired();

            builder.Property(x => x.DefaultLiteralEnum)
                .IsRequired();

            builder.OwnsMany(x => x.TestNullablityChildren, ConfigureTestNullablityChildren);

            builder.HasOne(x => x.NullabilityPeer)
                .WithMany()
                .HasForeignKey(x => x.NullabilityPeerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureTestNullablityChildren(OwnedNavigationBuilder<TestNullablity, TestNullablityChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.TestNullablityId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TestNullablityId)
                .IsRequired();
        }
    }
}