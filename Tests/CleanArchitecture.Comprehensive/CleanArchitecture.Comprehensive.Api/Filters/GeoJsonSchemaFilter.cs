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
        if (schema is OpenApiSchema concreteSchema && typeof(Geometry).IsAssignableFrom(context.Type))
        {
            concreteSchema.Format = "geojson";
            concreteSchema.Properties?.Clear();
            concreteSchema.Required?.Clear();
            concreteSchema.Description = "GeoJSON geometry — shape of 'coordinates' depends on the geometry type.";

            if (context.Type == typeof(Point))
            {
                concreteSchema.Example = new JsonObject
                    {
                        { "type", "Point" },
                        { "coordinates", new JsonArray { 1.0, 2.0 } }
                    };
            }
        }
    }
}