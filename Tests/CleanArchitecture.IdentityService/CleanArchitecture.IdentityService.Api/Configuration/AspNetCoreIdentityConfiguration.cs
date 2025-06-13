using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AspNetCoreIdentityConfiguration", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Api.Configuration
{
    public static class AspNetCoreIdentityConfiguration
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentityWithoutCookieAuth<ApplicationIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(
                options =>
                {
                    // Password settings.
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = false;
                });
        }
    }
}