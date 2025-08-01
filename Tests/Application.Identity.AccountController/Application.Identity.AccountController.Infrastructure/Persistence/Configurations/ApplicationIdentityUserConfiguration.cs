using System;
using Application.Identity.AccountController.Domain;
using Application.Identity.AccountController.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Application.Identity.AccountController.Infrastructure.Persistence.Configurations
{
    public class ApplicationIdentityUserConfiguration : IEntityTypeConfiguration<ApplicationIdentityUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationIdentityUser> builder)
        {
            builder.HasBaseType<IdentityUser<string>>();
            builder.Property(x => x.RefreshToken);

            builder.Property(x => x.RefreshTokenExpired);

            builder.OwnsOne(x => x.Address, ConfigureAddress);
        }

        public static void ConfigureAddress(OwnedNavigationBuilder<ApplicationIdentityUser, Address> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();
        }
    }
}