using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class CompanyContactSecondConfiguration : IEntityTypeConfiguration<CompanyContactSecond>
    {
        public void Configure(EntityTypeBuilder<CompanyContactSecond> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContactSecondId)
                .IsRequired();

            builder.HasOne(x => x.ContactSecond)
                .WithMany()
                .HasForeignKey(x => x.ContactSecondId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}