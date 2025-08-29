using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations
{
    public class FinalConfiguration : IEntityTypeConfiguration<Final>
    {
        public void Configure(EntityTypeBuilder<Final> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OneId)
                .IsRequired();

            builder.Property(x => x.TwoId)
                .IsRequired();

            builder.Property(x => x.ThreeId)
                .IsRequired();

            builder.Property(x => x.FourId)
                .IsRequired();

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}