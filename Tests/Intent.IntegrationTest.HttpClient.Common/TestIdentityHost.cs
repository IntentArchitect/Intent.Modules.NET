using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.HttpClient.Common;

public static class TestIdentityHost
{
    public const string IdentityServerUri = "http://localhost:{0}";

    static TestIdentityHost()
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    }

    public static async Task<IWebHost> SetupIdentityServer(
        ITestOutputHelper outputHelper = null,
        int portNumber = 5001)
    {
        var hostBuilder = new WebHostBuilder()
            .UseUrls(string.Format(IdentityServerUri, portNumber))
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
                            ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                            Claims = new List<ClientClaim>() { new ClientClaim("sub", "7869616D-2885-4B8D-847B-25AC5CFE511E") },
                            ClientClaimsPrefix = ""
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
                if (outputHelper != null)
                {
                    services.AddLogging((builder) => builder.AddXUnit(outputHelper));
                }
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