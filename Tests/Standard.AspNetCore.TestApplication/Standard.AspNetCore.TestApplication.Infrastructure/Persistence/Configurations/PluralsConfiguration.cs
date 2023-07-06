using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure.Persistence.Configurations
{
    public class PluralsConfiguration : IEntityTypeConfiguration<Plurals>
    {
        public void Configure(EntityTypeBuilder<Plurals> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}