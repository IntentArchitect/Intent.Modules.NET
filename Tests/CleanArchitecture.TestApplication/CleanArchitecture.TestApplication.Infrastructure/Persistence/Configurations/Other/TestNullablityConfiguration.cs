using CleanArchitecture.TestApplication.Domain.Entities.Other;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Other
{
    public class TestNullablityConfiguration : IEntityTypeConfiguration<TestNullablity>
    {
        public void Configure(EntityTypeBuilder<TestNullablity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MyEnum)
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