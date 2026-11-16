using EntityFrameworkCore.Postgres.EF11.Domain;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Infrastructure.Persistence.Configurations
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                    .IsRequired();

            builder.OwnsMany(x => x.Posts, ConfigurePosts);
        }

        public static void ConfigurePosts(OwnedNavigationBuilder<Blog, Post> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.BlogId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                    .IsRequired();

            builder.Property(x => x.PublishedOn)
                    .IsRequired();

            builder.Property(x => x.BlogId)
                    .IsRequired();
        }
    }
}