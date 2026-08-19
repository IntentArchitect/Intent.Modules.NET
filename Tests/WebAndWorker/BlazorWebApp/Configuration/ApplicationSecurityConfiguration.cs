using System.IdentityModel.Tokens.Jwt;
using BlazorWebApp.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using WebAndWorker.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.ApplicationSecurityConfiguration", Version = "1.0")]

namespace BlazorWebApp.Configuration
{
    public static class ApplicationSecurityConfiguration
    {
        public static IServiceCollection ConfigureApplicationSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddHttpContextAccessor();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options =>
                {
                    options.TokenValidationParameters.RoleClaimType = "roles";
                    options.TokenValidationParameters.NameClaimType = "name";
                }, identityOptions =>
                {
                    configuration.GetSection("AzureAd").Bind(identityOptions);
                });
            services.Configure<OpenIdConnectOptions>(
                OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters.RoleClaimType = "roles";
                    options.TokenValidationParameters.NameClaimType = "name";
                });

            services.AddAuthorization(ConfigureAuthorization);
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddHttpContextAccessor();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
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
            //options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("roles", "employee"));
            //options.AddPolicy("AdminOnly", policy => policy.RequireClaim("roles", "admin"));
        }
    }
}