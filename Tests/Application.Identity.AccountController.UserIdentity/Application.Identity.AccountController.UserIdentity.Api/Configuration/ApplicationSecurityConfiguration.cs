using System;
using System.IdentityModel.Tokens.Jwt;
using Application.Identity.AccountController.UserIdentity.Api.Services;
using Application.Identity.AccountController.UserIdentity.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.ApplicationSecurityConfiguration", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Api.Configuration
{
    public static class ApplicationSecurityConfiguration
    {
        public static IServiceCollection ConfigureApplicationSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddHttpContextAccessor();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = configuration.GetSection("JwtToken:Issuer").Get<string>(),
                            ValidateAudience = true,
                            ValidAudience = configuration.GetSection("JwtToken:Audience").Get<string>(),
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration.GetSection("JwtToken:SigningKey").Get<string>()!)),
                            NameClaimType = "sub"
                        };

                        options.TokenValidationParameters.RoleClaimType = "role";
                        options.SaveToken = true;
                    });

            services.AddAuthorization(ConfigureAuthorization);

            return services;
        }

        [IntentManaged(Mode.Ignore)]
        private static void ConfigureAuthorization(AuthorizationOptions options)
        {
            //Configure policies and other authorization options here. For example:
            //options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("role", "employee"));
            //options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "admin"));
        }
    }
}