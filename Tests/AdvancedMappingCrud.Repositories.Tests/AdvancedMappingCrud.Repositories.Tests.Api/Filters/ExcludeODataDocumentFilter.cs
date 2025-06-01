using System;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.OData.EntityFramework.ExcludeODataDocumentFilter", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Filters
{
    public class ExcludeODataDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var keepPaths = new[] { "/odata", "/odata/$metadata" };
            var keysToRemove = swaggerDoc.Paths.Keys
            .Where(
                path =>
                {
                    return path.StartsWith("/odata", StringComparison.OrdinalIgnoreCase) && !keepPaths.Contains(path, StringComparer.OrdinalIgnoreCase);
                });

            foreach (var key in keysToRemove)
            {
                swaggerDoc.Paths.Remove(key);
            }

            foreach (var keep in keepPaths)
            {
                if (!swaggerDoc.Paths.ContainsKey(keep))
                {
                    var openApiPathItem = new OpenApiPathItem
                    {
                        Description = "OData service endpoint"
                    };
                    swaggerDoc.Paths.Add(keep, openApiPathItem);
                }
            }
        }
    }
}