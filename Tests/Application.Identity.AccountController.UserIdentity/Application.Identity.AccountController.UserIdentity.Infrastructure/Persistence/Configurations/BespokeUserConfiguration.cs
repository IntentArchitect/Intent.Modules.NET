using System;
using Application.Identity.AccountController.UserIdentity.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Infrastructure.Persistence.Configurations
{
    public class BespokeUserConfiguration : IEntityTypeConfiguration<BespokeUser>
    {
        public void Configure(EntityTypeBuilder<BespokeUser> builder)
        {
            builder.HasBaseType<IdentityUser<string>>();

            builder.Property(x => x.RefreshToken);

            builder.Property(x => x.RefreshTokenExpired);

            builder.Property(x => x.FirstName)
                .IsRequired();

            builder.Property(x => x.LastName)
                .IsRequired();
        }
    }
}