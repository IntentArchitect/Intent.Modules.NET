using System.Net;
using System.Text.Json.Nodes;

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.OpenApi
{
    /// <summary>
    /// Proves the HTTP QUERY endpoint is described in the Swashbuckle document as an OpenAPI 3.2 "query" operation.
    /// </summary>
    [Collection("SharedContainer")]
    public class OpenApiDocumentTests : BaseIntegrationTest
    {
        private const string DocumentUri = "swagger/v1/swagger.json";
        private const string SearchPath = "/api/customers/search";

        public OpenApiDocumentTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Document_DeclaresOpenApi32()
        {
            var document = await GetDocumentAsync();

            // The native "query" operation only exists from OpenAPI 3.2.
            Assert.Equal("3.2.0", document["openapi"]!.GetValue<string>());
        }

        [Fact]
        public async Task Document_DescribesSearchCustomersAsQueryOperation()
        {
            var document = await GetDocumentAsync();

            var pathItem = document["paths"]?[SearchPath];
            Assert.NotNull(pathItem);
            var operation = pathItem!["query"];
            Assert.NotNull(operation);
            Assert.Null(pathItem["post"]);

            var bodySchema = Resolve(document, operation!["requestBody"]?["content"]?["application/json"]?["schema"]);
            Assert.NotNull(bodySchema["properties"]?["lastName"]);

            var responseSchema = Resolve(document, operation["responses"]?["200"]?["content"]?["application/json"]?["schema"]);
            Assert.Equal("array", responseSchema["type"]!.GetValue<string>());
            Assert.EndsWith(".CustomerDto", responseSchema["items"]?["$ref"]?.GetValue<string>());
        }

        private async Task<JsonObject> GetDocumentAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var response = await CreateClient().GetAsync(DocumentUri, cancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!.AsObject();
        }

        private static JsonObject Resolve(JsonObject document, JsonNode? schema)
        {
            Assert.NotNull(schema);
            var reference = schema!["$ref"]?.GetValue<string>();
            if (reference is null)
            {
                return schema.AsObject();
            }

            var name = reference.Split('/').Last();
            var target = document["components"]?["schemas"]?[name];
            Assert.True(target is not null, $"Missing component schema '{name}'");
            return target!.AsObject();
        }
    }
}
