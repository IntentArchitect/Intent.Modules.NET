using System.Net;
using System.Text.Json.Nodes;

namespace HttpQuery.Controllers.Swashbuckle.Application.IntegrationTests.OpenApi
{
    /// <summary>
    /// Proves the QUERY endpoints are described in the Swashbuckle document, and that the document is still valid.
    /// </summary>
    [Collection("SharedContainer")]
    public class OpenApiDocumentTests : BaseIntegrationTest
    {
        private const string DocumentUri = "swagger/v1/swagger.json";

        public OpenApiDocumentTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Document_DescribesBodyOnlyQueryOperation()
        {
            var document = await GetDocumentAsync();

            var operation = QueryOperation(document, "/api/products/search");
            var bodySchema = ResolveBodySchema(document, operation);
            Assert.NotNull(bodySchema["properties"]?["nameContains"]);
            Assert.NotNull(bodySchema["properties"]?["categories"]);
            AssertReturnsProductDtoArray(document, operation);
            AssertTagsDeclared(document, operation);
            Assert.Null(document["paths"]!["/api/products/search"]!["post"]);
        }

        [Fact]
        public async Task Document_DescribesRouteAndBodyQueryOperation()
        {
            var document = await GetDocumentAsync();

            var operation = QueryOperation(document, "/api/categories/{category}/products/search");
            var routeParameter = operation["parameters"]!.AsArray().Single(p => p!["name"]!.GetValue<string>() == "category");
            Assert.Equal("path", routeParameter!["in"]!.GetValue<string>());
            var bodySchema = ResolveBodySchema(document, operation);
            // Same as the generated POST endpoint: the controller requires the body's Category to match the route,
            // so the generated document keeps it in the body schema too (pre-existing, not QUERY-specific).
            Assert.NotNull(bodySchema["properties"]?["category"]);
            Assert.NotNull(bodySchema["properties"]?["nameContains"]);
            AssertReturnsProductDtoArray(document, operation);
        }

        /// <summary>
        /// The native "query" operation only exists from OpenAPI 3.2, so the document must say 3.2. Microsoft.OpenApi 2.x
        /// (used by Swashbuckle 10) cannot read 3.2; Spike/scripts/validate-openapi.cs checks it with Microsoft.OpenApi 3.x.
        /// </summary>
        [Fact]
        public async Task Document_DeclaresOpenApi32()
        {
            var document = await GetDocumentAsync();

            Assert.Equal("3.2.0", document["openapi"]!.GetValue<string>());
        }

        private async Task<JsonObject> GetDocumentAsync()
        {
            var ct = TestContext.Current.CancellationToken;
            var response = await CreateClient().GetAsync(DocumentUri, ct);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return JsonNode.Parse(await response.Content.ReadAsStringAsync(ct))!.AsObject();
        }

        /// <summary>
        /// The QUERY operation sits under OpenAPI 3.2's native "query" key on the path item.
        /// </summary>
        private static JsonObject QueryOperation(JsonObject document, string path)
        {
            var pathItem = document["paths"]?[path];
            Assert.NotNull(pathItem);
            var operation = pathItem!["query"];
            Assert.NotNull(operation);
            return operation!.AsObject();
        }

        private static JsonObject ResolveBodySchema(JsonObject document, JsonObject operation)
        {
            var schema = operation["requestBody"]?["content"]?["application/json"]?["schema"];
            Assert.NotNull(schema);
            return Resolve(document, schema!.AsObject());
        }

        private static void AssertReturnsProductDtoArray(JsonObject document, JsonObject operation)
        {
            var schema = operation["responses"]?["200"]?["content"]?["application/json"]?["schema"];
            Assert.NotNull(schema);
            var resolved = Resolve(document, schema!.AsObject());
            Assert.Equal("array", resolved["type"]!.GetValue<string>());
            var itemRef = resolved["items"]?["$ref"]?.GetValue<string>();
            Assert.EndsWith(".ProductDto", itemRef);
            Assert.NotNull(Resolve(document, resolved["items"]!.AsObject())["properties"]);
        }

        private static void AssertTagsDeclared(JsonObject document, JsonObject operation)
        {
            var declared = document["tags"]?.AsArray().Select(t => t!["name"]!.GetValue<string>()).ToHashSet() ?? [];
            foreach (var tag in operation["tags"]?.AsArray() ?? [])
            {
                Assert.Contains(tag!.GetValue<string>(), declared);
            }
        }

        private static JsonObject Resolve(JsonObject document, JsonObject schema)
        {
            var reference = schema["$ref"]?.GetValue<string>();
            if (reference is null)
            {
                return schema;
            }

            var name = reference.Split('/').Last();
            var target = document["components"]?["schemas"]?[name];
            Assert.True(target is not null, $"Missing component schema '{name}'");
            return target!.AsObject();
        }
    }
}
