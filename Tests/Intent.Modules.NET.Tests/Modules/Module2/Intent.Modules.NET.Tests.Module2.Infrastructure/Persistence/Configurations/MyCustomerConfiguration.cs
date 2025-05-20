using Intent.Modules.NET.Tests.Module2.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Infrastructure.Persistence.Configurations
{
    public class MyCustomerConfiguration : IEntityTypeConfiguration<MyCustomer>
    {
        public void Configure(EntityTypeBuilder<MyCustomer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}