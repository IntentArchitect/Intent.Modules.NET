using Microsoft.OpenApi;

namespace HttpQuery.Controllers.Swashbuckle.Api.Filters
{
    public enum HttpQueryOpenApiShape
    {
        /// <summary>Shape A: <c>paths./x.query</c>. Only valid from OpenAPI 3.2.</summary>
        NativeQueryKey,

        /// <summary>Shape B: <c>paths./x.x-oai-additionalOperations.QUERY</c>, valid in OpenAPI 3.0/3.1.</summary>
        AdditionalOperationsExtension
    }

    /// <summary>
    /// Writes a path item's non-standard operations under <c>x-oai-additionalOperations</c>, the extension Microsoft.OpenApi 3
    /// and ASP.NET Core 11 use for OpenAPI 3.0/3.1 documents (OpenAPI 3.2 calls the same map <c>additionalOperations</c>).
    /// </summary>
    public sealed class AdditionalOperationsExtension : IOpenApiExtension
    {
        public const string Name = "x-oai-additionalOperations";

        public Dictionary<string, OpenApiOperation> Operations { get; } = new(StringComparer.Ordinal);

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();
            foreach (var (method, operation) in Operations)
            {
                writer.WritePropertyName(method);
                if (specVersion == OpenApiSpecVersion.OpenApi3_1)
                {
                    operation.SerializeAsV31(writer);
                }
                else
                {
                    operation.SerializeAsV3(writer);
                }
            }
            writer.WriteEndObject();
        }
    }
}
