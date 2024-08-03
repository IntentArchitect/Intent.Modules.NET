using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Geometries;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.NetTopologySuite.GeoJsonSchemaSwaggerFilter", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.Filters;

public class GeoJsonSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Point))
        {
            schema.Format = "geojson";
            schema.Example = new OpenApiObject
                {
                    { "type", new OpenApiString("Point") },
                    { "coordinates", new OpenApiArray { new OpenApiDouble(1.0), new OpenApiDouble(2.0) } }
                };
        }
    }
}