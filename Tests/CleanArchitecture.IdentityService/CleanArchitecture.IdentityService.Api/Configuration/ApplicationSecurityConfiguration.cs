using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.IdentityService.Api.Services;
using CleanArchitecture.IdentityService.Application.Common.Interfaces;
using CleanArchitecture.IdentityService.Application.Interfaces;
using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Infrastructure.Implementation.Identity;
using CleanArchitecture.IdentityService.Infrastructure.Options;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.ApplicationSecurityConfiguration", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Api.Configuration
{
    public static class ApplicationSecurityConfiguration
    {
        public static IServiceCollection ConfigureApplicationSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IIdentityServiceManager, IdentityServiceManager>();
            services.AddIdentity<ApplicationIdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationIdentityUser>>(IdentityConstants.BearerScheme)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddHttpContextAccessor();
            services.AddScoped<CleanArchitecture.IdentityService.Application.Interfaces.IEmailSender<ApplicationIdentityUser>, EmailSender>();
            services.Configure<EmailSenderOptions>(configuration.GetSection("EmailSender"));
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddHttpContextAccessor();

            services.AddAuthentication(options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Audience = configuration["JwtToken:Audience"];

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = configuration["JwtToken:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = configuration["JwtToken:Audience"],
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["JwtToken:SigningKey"])),
                            RoleClaimType = "role"
                        };

                        options.SaveToken = true;
                    });

            services.AddAuthorization(ConfigureAuthorization);

            return services;
        }

        [IntentManaged(Mode.Ignore)]
        private static void ConfigureAuthorization(AuthorizationOptions options)
        {
            // Configure policies and other authorization options here. For example:
            // options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("role", "employee"));
            // options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "admin"));
        }
    }
}