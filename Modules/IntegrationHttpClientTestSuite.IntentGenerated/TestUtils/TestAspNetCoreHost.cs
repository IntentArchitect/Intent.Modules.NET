using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationHttpClientTestSuite.IntentGenerated.TestUtils;

public static class TestAspNetCoreHost
{
    public const string BackendFacingServerUri = "http://localhost:5002";

    public static async Task<IWebHost> SetupApiServer(Action<IServiceCollection> serviceConfig = null)
    {
        var hostBuilder = new WebHostBuilder()
            .UseUrls(BackendFacingServerUri)
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = TestIdentityHost.IdentityServerUri;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                    });
                services.AddAuthorization();
                serviceConfig?.Invoke(services);
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