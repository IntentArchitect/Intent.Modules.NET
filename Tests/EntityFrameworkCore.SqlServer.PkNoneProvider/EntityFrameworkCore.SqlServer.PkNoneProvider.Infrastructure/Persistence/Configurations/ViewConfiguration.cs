using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations
{
    public class ViewConfiguration : IEntityTypeConfiguration<View>
    {
        public void Configure(EntityTypeBuilder<View> builder)
        {
            builder.ToView("Views", "myapp");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();
        }
    }
}