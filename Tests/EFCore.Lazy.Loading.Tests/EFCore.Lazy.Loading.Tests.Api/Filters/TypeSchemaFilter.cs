using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.TypeSchemaFilter", Version = "1.0")]

namespace EFCore.Lazy.Loading.Tests.Api.Filters
{
    public class TypeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(TimeSpan) || context.Type == typeof(TimeSpan?))
            {
                schema.Example = new OpenApiString("00:00:00"); // Set your desired format here
                schema.Type = "string"; // Override the default representation to be a string
            }
        }
    }
}