using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using EntityFrameworkCore.Repositories.TestApplication.Api.Services;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Security.JWT.ConfigurationJWTAuthentication", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Api.Configuration
{
    public static class JWTAuthenticationConfiguration
    {
        // Use '[IntentManaged(Mode.Ignore)]' on this method should you want to deviate from the standard JWT token approach
        public static IServiceCollection ConfigureJWTSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration.GetSection("Security.Bearer:Authority").Get<string>();
                    options.Audience = configuration.GetSection("Security.Bearer:Audience").Get<string>();

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