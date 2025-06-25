using CleanArchitecture.IdentityService.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Infrastructure.Persistence.Configurations
{
    public class ApplicationIdentityUserConfiguration : IEntityTypeConfiguration<ApplicationIdentityUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationIdentityUser> builder)
        {
            builder.HasBaseType<IdentityUser<string>>();

            builder.Property(x => x.RefreshToken);

            builder.Property(x => x.RefreshTokenExpired);
        }
    }
}