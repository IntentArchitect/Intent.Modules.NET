using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRoot5Configuration : IEntityTypeConfiguration<AggregateRoot5>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot5> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.AggregateRoot5EntityWithRepo, ConfigureAggregateRoot5EntityWithRepo)
                .Navigation(x => x.AggregateRoot5EntityWithRepo).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAggregateRoot5EntityWithRepo(OwnedNavigationBuilder<AggregateRoot5, AggregateRoot5EntityWithRepo> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);
        }
    }
}