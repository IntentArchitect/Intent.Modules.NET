using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence.Configurations
{
    public class EfAffiliateConfiguration : IEntityTypeConfiguration<EfAffiliate>
    {
        public void Configure(EntityTypeBuilder<EfAffiliate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}