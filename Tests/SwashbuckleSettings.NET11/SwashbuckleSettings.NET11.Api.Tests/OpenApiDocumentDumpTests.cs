using Microsoft.AspNetCore.Mvc.Testing;
using SwashbuckleSettings.NET11.Api;
using Xunit;

namespace SwashbuckleSettings.NET11.Api.Tests
{
    /// <summary>
    /// Writes the served OpenAPI document to OPENAPI_DUMP_PATH when that variable is set, so a run can keep it as evidence.
    /// </summary>
    public class OpenApiDocumentDumpTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task OpenApiDocument_IsServed()
        {
            var client = factory.CreateClient();
            var json = await client.GetStringAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);
            Assert.False(string.IsNullOrWhiteSpace(json));

            var path = Environment.GetEnvironmentVariable("OPENAPI_DUMP_PATH");
            if (!string.IsNullOrEmpty(path))
            {
                await File.WriteAllTextAsync(path, json, TestContext.Current.CancellationToken);
            }
        }
    }
}
