using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence.Configurations
{
    public class EfClientConfiguration : IEntityTypeConfiguration<EfClient>
    {
        public void Configure(EntityTypeBuilder<EfClient> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.AffiliateId)
                .IsRequired();

            builder.OwnsMany(x => x.Sites, ConfigureSites);

            builder.HasOne(x => x.Affiliate)
                .WithMany()
                .HasForeignKey(x => x.AffiliateId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void ConfigureSites(OwnedNavigationBuilder<EfClient, EfSite> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ClientId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.ClientId)
                .IsRequired();

            builder.OwnsMany(x => x.Departments, ConfigureDepartments);
        }

        public static void ConfigureDepartments(OwnedNavigationBuilder<EfSite, EfDepartment> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.SiteId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.SiteId)
                .IsRequired();
        }
    }
}