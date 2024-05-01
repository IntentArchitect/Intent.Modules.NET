using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Models;
using OutputCachingRedis.Tests.Api.Controllers.FileTransfer;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.BinaryContentFilter", Version = "1.0")]

namespace OutputCachingRedis.Tests.Api.Filters
{
    public class BinaryContentFilter : IOperationFilter
    {
        /// <summary>
        /// Configures operations decorated with the <see cref="BinaryContentAttribute" />.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attribute = context.MethodInfo.GetCustomAttributes(typeof(BinaryContentAttribute), false).FirstOrDefault();

            if (attribute == null)
            {
                return;
            }
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Content-Disposition",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                },
                Description = "e.g. form-data; name=\"file\"; filename=example.txt"
            });
            operation.RequestBody = new OpenApiRequestBody() { Required = true };
            operation.RequestBody.Content.Add("application/octet-stream", new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = "string",
                    Format = "binary",
                },
            });
        }
    }
}