using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Testing;
using Scalar.NET11.Bearer.Api;
using Xunit;

namespace Scalar.NET11.Bearer.Api.Tests
{
    /// <summary>
    /// Checks the Microsoft.AspNetCore.OpenApi document the app serves on .NET 11 (Microsoft.OpenApi 3.x).
    /// </summary>
    public class OpenApiDocumentTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string MergePatchContentType = "application/merge-patch+json";

        [Fact]
        public async Task UpdateOperation_HidesRouteIdFromBodySchema()
        {
            var document = await GetDocumentAsync();
            var operation = document["paths"]!["/api/widgets/{id}"]!["put"]!;

            Assert.Contains(operation["parameters"]!.AsArray(), p => (string?)p!["name"] == "id" && (string?)p["in"] == "path");

            var content = operation["requestBody"]!["content"]!.AsObject();
            Assert.NotEmpty(content);
            foreach (var (_, mediaType) in content)
            {
                var properties = Resolve(document, mediaType!["schema"]!)["properties"]!.AsObject();
                Assert.False(properties.ContainsKey("id"), "The PUT body schema still documents the route parameter 'id'.");
                Assert.True(properties.ContainsKey("name"));
            }
        }

        [Fact]
        public async Task QueryOperation_IsDescribedOnceWithItsRequestBody()
        {
            var document = await GetDocumentAsync();
            var pathItem = document["paths"]!["/api/widgets/search"]!.AsObject();

            var queryOperations = new List<JsonNode>();
            if (pathItem["query"] is { } native)
            {
                queryOperations.Add(native);
            }
            if (pathItem["x-oai-additionalOperations"]?["QUERY"] is { } extension)
            {
                queryOperations.Add(extension);
            }

            var operation = Assert.Single(queryOperations);
            var content = operation["requestBody"]!["content"]!.AsObject();
            Assert.True(content.ContainsKey("application/json"));
            Assert.EndsWith("WidgetSearchCriteriaDto", (string?)content["application/json"]!["schema"]!["$ref"]);
        }

        [Fact]
        public async Task QueryOperation_IsANativeQueryOperation_WhenTheDocumentIsOpenApi32()
        {
            var document = await GetDocumentAsync();
            var version = (string?)document["openapi"];
            Assert.StartsWith("3.2", version);

            // OpenAPI 3.2 has a native "query" operation; "x-oai-additionalOperations" is how Microsoft.OpenApi writes it into a 3.1 document.
            var pathItem = document["paths"]!["/api/widgets/search"]!.AsObject();
            Assert.True(pathItem.ContainsKey("query"),
                $"The document is labelled {version} but the QUERY operation is written as: {pathItem.ToJsonString()}");
        }

        [Fact]
        public async Task UploadOperation_IsDescribed()
        {
            // Scalar has no transformer for [BinaryContent], so no request body is documented; the Swashbuckle app covers that filter.
            var document = await GetDocumentAsync();
            var operation = document["paths"]!["/api/widgets/images"]!["post"]!;

            Assert.NotNull(operation["responses"]!["201"]);
        }

        [Fact]
        public async Task PatchOperation_UsesMergePatchContentType()
        {
            var document = await GetDocumentAsync();
            var operation = document["paths"]!["/api/widgets/{id}"]!["patch"]!;

            var content = operation["requestBody"]!["content"]!.AsObject();
            Assert.Equal([MergePatchContentType], content.Select(c => c.Key));

            var properties = Resolve(document, content[MergePatchContentType]!["schema"]!)["properties"]!.AsObject();
            Assert.True(properties.ContainsKey("name"));
            Assert.False(properties.ContainsKey("patchExecutor"));
        }

        private async Task<JsonNode> GetDocumentAsync()
        {
            var client = factory.CreateClient();
            var json = await client.GetStringAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
            return JsonNode.Parse(json)!;
        }

        private static JsonNode Resolve(JsonNode document, JsonNode schema)
        {
            if ((string?)schema["$ref"] is not { } reference)
            {
                return schema;
            }

            var node = document;
            foreach (var segment in reference.TrimStart('#', '/').Split('/'))
            {
                node = node[segment]!;
            }
            return node;
        }
    }
}
