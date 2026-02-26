using System.Text.Json.Nodes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi;
using NetTopologySuite.Geometries;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.NetTopologySuite.GeoJsonSchemaSwaggerFilter", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.Filters;

public class GeoJsonSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is OpenApiSchema concreteSchema && context.Type == typeof(Point))
        {
            concreteSchema.Format = "geojson";
            concreteSchema.Example = new JsonObject
                {
                    { "type", "Point" },
                    { "coordinates", new JsonArray { 1.0, 2.0 } }
                };
        }
    }
}