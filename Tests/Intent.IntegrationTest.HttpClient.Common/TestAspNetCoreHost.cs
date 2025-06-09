using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Infrastructure.Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.HttpClient.Common;

public static class TestAspNetCoreHost
{
    public const string BackendFacingServerUri = "http://localhost:{0}";

    public static async Task<IWebHost> SetupApiServer(
        ITestOutputHelper outputHelper = null, 
        Action<IServiceCollection> serviceConfig = null, 
        Assembly assemblyWithControllers = null,
        int apiPortNumber = 5002,
        int idPortNumber = 5001)
    {
        var hostBuilder = new WebHostBuilder()
            .UseUrls(string.Format(BackendFacingServerUri, apiPortNumber))
            .ConfigureServices(services =>
            {
                if (assemblyWithControllers != null)
                {
                    services.AddControllers().AddApplicationPart(assemblyWithControllers);
                }
                else
                {
                    services.AddControllers();
                }
                JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = string.Format(TestIdentityHost.IdentityServerUri, idPortNumber);
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            RequireSignedTokens = false,
                            SignatureValidator = (token, parameters) =>
                            {
                                var handler = new JsonWebTokenHandler();
                                var jwt = handler.ReadJsonWebToken(token);
                                return jwt;
                            }
                        };
                        options.TokenValidationParameters.RoleClaimType = "role";
                        options.SaveToken = true;
                    });
                services.AddAuthorization();
                if (outputHelper != null)
                {
                    services.AddLogging((builder) => builder.AddXUnit(outputHelper));
                }
                serviceConfig?.Invoke(services);

                services.AddDistributedMemoryCache();
                services.AddSingleton<IDistributedCacheWithUnitOfWork, DistributedCacheWithUnitOfWork>();
                services.AddSingleton<Standard.AspNetCore.TestApplication.Application.Common.Interfaces.IDistributedCacheWithUnitOfWork, Standard.AspNetCore.TestApplication.Infrastructure.Caching.DistributedCacheWithUnitOfWork>();
            })
            .Configure(app =>
            {
                app.UseDeveloperExceptionPage();
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