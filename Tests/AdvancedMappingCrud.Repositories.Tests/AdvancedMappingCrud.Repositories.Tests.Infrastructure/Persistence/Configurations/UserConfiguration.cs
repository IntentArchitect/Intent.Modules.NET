using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasBaseType<Person>();

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.QuoteId)
                .IsRequired();

            builder.HasOne(x => x.Quote)
                .WithMany()
                .HasForeignKey(x => x.QuoteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}