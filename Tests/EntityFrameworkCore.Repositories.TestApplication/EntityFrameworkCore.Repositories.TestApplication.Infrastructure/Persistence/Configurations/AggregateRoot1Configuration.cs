using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRoot1Configuration : IEntityTypeConfiguration<AggregateRoot1>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot1> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Tag)
                .IsRequired()
                .HasMaxLength(125)
                .HasComment(@"Here is a multi
line comment that
is supposed to work for ""HasComment()""
and has some quotes included");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}