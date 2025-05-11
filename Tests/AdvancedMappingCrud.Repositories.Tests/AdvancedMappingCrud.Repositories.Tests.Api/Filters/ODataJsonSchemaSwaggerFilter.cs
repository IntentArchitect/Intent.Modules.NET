using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.OData.EntityFramework.ODataJsonSchemaSwaggerFilter", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Filters
{
    public class ODataJsonSchemaSwaggerFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var hasDomainEvents = schema.Properties.ContainsKey("domainEvents");

            if (hasDomainEvents && context.Type.GetInterfaces().Any(i => i.Name == "IHasDomainEvent"))
            {
                schema.Properties.Remove("domainEvents");
            }
        }
    }
}