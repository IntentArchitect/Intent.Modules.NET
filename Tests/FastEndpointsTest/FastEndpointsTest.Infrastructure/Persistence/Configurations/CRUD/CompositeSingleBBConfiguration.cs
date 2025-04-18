using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FastEndpointsTest.Infrastructure.Persistence.Configurations.CRUD
{
    public class CompositeSingleBBConfiguration : IEntityTypeConfiguration<CompositeSingleBB>
    {
        public void Configure(EntityTypeBuilder<CompositeSingleBB> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompositeAttr)
                .IsRequired();
        }
    }
}