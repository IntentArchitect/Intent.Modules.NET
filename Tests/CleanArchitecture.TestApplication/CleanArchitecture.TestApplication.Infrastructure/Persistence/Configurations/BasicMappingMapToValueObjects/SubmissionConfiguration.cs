using CleanArchitecture.TestApplication.Domain.BasicMappingMapToValueObjects;
using CleanArchitecture.TestApplication.Domain.Entities.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.BasicMappingMapToValueObjects
{
    public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SubmissionType)
                .IsRequired();

            builder.OwnsMany(x => x.Items, ConfigureItems);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureItems(OwnedNavigationBuilder<Submission, Item> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Key)
                .IsRequired();

            builder.Property(x => x.Value)
                .IsRequired();
        }
    }
}