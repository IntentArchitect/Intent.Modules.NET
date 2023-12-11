using System;
using Application.Identity.AccountController.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.ApplicationIdentityUserConfiguration", Version = "1.0")]

namespace Application.Identity.AccountController.Infrastructure.Persistence.Configurations
{
    public class ApplicationIdentityUserConfiguration : IEntityTypeConfiguration<ApplicationIdentityUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationIdentityUser> builder)
        {
            builder.Property(x => x.RefreshToken);
            builder.Property(x => x.RefreshTokenExpired);
        }
    }
}