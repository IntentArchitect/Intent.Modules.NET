using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Contracts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations
{
    public class AccountHolderPersonConfiguration : IEntityTypeConfiguration<AccountHolderPerson>
    {
        public void Configure(EntityTypeBuilder<AccountHolderPerson> builder)
        {
            builder.HasNoKey().ToView(null);
            builder.Property(x => x.Height).HasPrecision(17, 3);
        }
    }
}