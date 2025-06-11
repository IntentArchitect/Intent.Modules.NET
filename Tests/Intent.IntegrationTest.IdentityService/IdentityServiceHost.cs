using CleanArchitecture.IdentityService.Api.Controllers;
using CleanArchitecture.IdentityService.Api.Services;
using CleanArchitecture.IdentityService.Application.Interfaces;
using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Infrastructure.Implementation.Identity;
using CleanArchitecture.IdentityService.Infrastructure.Options;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.IdentityService
{
    internal class IdentityServiceHost
    {
        public const string IdentityServiceUri = "http://localhost:{0}";

        public static async Task<IWebHost> SetupIdentityService(CleanArchitecture.IdentityService.Application.Interfaces.IEmailSender<ApplicationIdentityUser> emailSender,
            ITestOutputHelper outputHelper = null,
            int portNumber = 5001)
        {
            var hostBuilder = new WebHostBuilder()
                .UseUrls(string.Format(IdentityServiceUri, portNumber))
                .ConfigureServices(services =>
                {
                    if (outputHelper != null)
                    {
                        services.AddLogging((builder) => builder.AddXunit(outputHelper));
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("testContext"));
                    services.AddSingleton<ITokenService, TokenService>();
                    services.AddControllers().AddApplicationPart(typeof(IdentityController).Assembly);
                    services.AddScoped<IIdentityServiceManager, IdentityServiceManager>();
                    services.AddIdentity<ApplicationIdentityUser, IdentityRole>()
                        .AddDefaultTokenProviders()
                        .AddTokenProvider<DataProtectorTokenProvider<ApplicationIdentityUser>>(IdentityConstants.BearerScheme)
                        .AddEntityFrameworkStores<ApplicationDbContext>();
                    services.AddHttpContextAccessor();

                    var configData = new Dictionary<string, string>
                    {
                        ["JwtToken:SigningKey"] = "aHHDYCTvyZVbdcGgaDvL+T6837pHCkciU0rLvUbE9a4=",
                        ["JwtToken:Audience"] = "TestAudience",
                        ["JwtToken:RefreshTokenExpiryTimeSpan"] = "3.00:00:00",
                        ["JwtToken:AuthTokenExpiryTimeSpan"] = "02:00:00",
                        ["JwtToken:Issuer"] = string.Format(IdentityServiceUri, portNumber)
                    };

                    // Build configuration
                    var configuration = new ConfigurationBuilder()
                        .AddInMemoryCollection(configData)
                        .Build();

                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddScoped<CleanArchitecture.IdentityService.Application.Interfaces.IEmailSender<ApplicationIdentityUser>>(x => emailSender);

                    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
                    services.AddHttpContextAccessor();
                        services.AddAuthentication(options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
                    .AddJwtBearer(
                        JwtBearerDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.Audience = "TestApp";

                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = string.Format(IdentityServiceUri, portNumber),
                                ValidateAudience = true,
                                ValidAudience = "test",
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("aHHDYCTvyZVbdcGgaDvL+T6837pHCkciU0rLvUbE9a4=")),
                                RoleClaimType = "role"
                            };

                            options.SaveToken = true;
                        });
                })
                .Configure((app) =>
                {
                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });

            var host = hostBuilder.UseKestrel().Build();
            await host.StartAsync();
            return host;
        }
    }

    internal class JwtToken
    {
        public string Audience { get; set; }
        public string SigningKey { get; set; }
        public string AuthTokenExpiryTimeSpan { get; set; }
        public string RefreshTokenExpiryTimeSpan { get; set; }
    }
    
}
