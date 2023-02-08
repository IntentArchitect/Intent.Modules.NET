using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.HttpClient.TestUtils;

public static class TestAspNetCoreHost
{
    public const string BackendFacingServerUri = "http://localhost:5002";

    public static async Task<IWebHost> SetupApiServer(ITestOutputHelper outputHelper = null, Action<IServiceCollection> serviceConfig = null, Assembly assemblyWithControllers = null)
    {
        var hostBuilder = new WebHostBuilder()
            .UseUrls(BackendFacingServerUri)
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
                if (outputHelper != null)
                {
                    services.AddLogging((builder) => builder.AddXUnit(outputHelper));
                }
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