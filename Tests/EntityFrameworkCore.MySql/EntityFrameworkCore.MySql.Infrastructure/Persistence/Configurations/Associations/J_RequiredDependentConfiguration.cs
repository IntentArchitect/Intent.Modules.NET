using EntityFrameworkCore.MySql.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.Associations
{
    public class J_RequiredDependentConfiguration : IEntityTypeConfiguration<J_RequiredDependent>
    {
        public void Configure(EntityTypeBuilder<J_RequiredDependent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReqDepAttr)
                .IsRequired();
        }
    }
}