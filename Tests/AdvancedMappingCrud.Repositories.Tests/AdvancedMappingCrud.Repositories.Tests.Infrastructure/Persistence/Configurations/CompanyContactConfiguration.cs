using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class CompanyContactConfiguration : IEntityTypeConfiguration<CompanyContact>
    {
        public void Configure(EntityTypeBuilder<CompanyContact> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContactId)
                .IsRequired();

            builder.HasOne(x => x.Contact)
                .WithMany()
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}