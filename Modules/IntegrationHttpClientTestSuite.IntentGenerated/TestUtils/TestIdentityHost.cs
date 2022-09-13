using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationHttpClientTestSuite.IntentGenerated.TestUtils;

public static class TestIdentityHost
{
    public const string IdentityServerUri = "http://localhost:5001";

    static TestIdentityHost()
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    }

    public static async Task<IWebHost> SetupIdentityServer()
    {
        var hostBuilder = new WebHostBuilder()
            .UseUrls(IdentityServerUri)
            .ConfigureServices(services =>
            {
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryClients(new[]
                    {
                        new Client
                        {
                            ClientName = "Service Account",
                            ClientId = "ServiceAccountClient",
                            AccessTokenType = AccessTokenType.Jwt,
                            RequireClientSecret = true,
                            AllowedGrantTypes = new List<string> { GrantType.ClientCredentials },
                            AllowedScopes = new List<string> { "api" },
                            ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) }
                        }
                    })
                    .AddInMemoryApiResources(new[]
                    {
                        new ApiResource("api")
                    })
                    .AddInMemoryApiScopes(new[]
                    {
                        new ApiScope("api")
                    })
                    ;
            })
            .Configure((app) =>
            {
                app.UseDeveloperExceptionPage();
                app.UseIdentityServer();
            });

        var host = hostBuilder.UseKestrel().Build();
        await host.StartAsync();
        return host;
    }
}