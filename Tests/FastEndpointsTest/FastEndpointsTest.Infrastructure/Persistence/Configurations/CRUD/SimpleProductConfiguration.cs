using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FastEndpointsTest.Infrastructure.Persistence.Configurations.CRUD
{
    public class SimpleProductConfiguration : IEntityTypeConfiguration<SimpleProduct>
    {
        public void Configure(EntityTypeBuilder<SimpleProduct> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Value)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}