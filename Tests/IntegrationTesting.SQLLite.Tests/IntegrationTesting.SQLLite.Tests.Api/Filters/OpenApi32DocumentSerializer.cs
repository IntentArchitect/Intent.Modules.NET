using System.Text.Json.Nodes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Swagger;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.OpenApi32DocumentSerializer", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Api.Filters
{
    /// <summary>
    /// Writes the document as OpenAPI 3.1 and labels it 3.2.0, the first version with a native <c>query</c> operation.
    /// <para>
    /// OpenAPI 3.2 only adds to 3.1, so a 3.1 document plus <c>query</c> operations is a valid 3.2 document. Swashbuckle 10
    /// (Microsoft.OpenApi 2.x) has no 3.2 writer; drop this once Swashbuckle writes OpenAPI 3.2 itself.
    /// </para>
    /// </summary>
    public sealed class OpenApi32DocumentSerializer : ISwaggerDocumentSerializer
    {
        private const string Version = "3.2.0";

        public void SerializeDocument(OpenApiDocument document, IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            using var buffer = new StringWriter();
            document.SerializeAsV31(new OpenApiJsonWriter(buffer));

            var json = JsonNode.Parse(buffer.ToString())!.AsObject();
            json["openapi"] = Version;
            writer.WriteRaw(json.ToJsonString());
        }
    }
}