using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi;
using Morcatko.AspNetCore.JsonMergePatch;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchOperationFilter", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Api.Filters
{
    public class JsonMergePatchOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.HttpMethod != "PATCH")
            {
                return;
            }

            if (operation.RequestBody?.Content == null)
            {
                return;
            }

            var patchParam = context.ApiDescription.ParameterDescriptions
                .FirstOrDefault(param => param.Type.IsGenericType && param.Type.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>));

            if (patchParam == null)
            {
                return;
            }

            var payloadType = patchParam.Type.GetGenericArguments()[0];

            var payloadSchemaReference = context.SchemaGenerator.GenerateSchema(payloadType, context.SchemaRepository) as OpenApiSchemaReference;

            if (payloadSchemaReference == null || string.IsNullOrEmpty(payloadSchemaReference.Reference.Id))
            {
                return;
            }

            if (context.SchemaRepository.Schemas.TryGetValue(payloadSchemaReference.Reference.Id, out var actualSchema))
            {
                actualSchema?.Required?.Remove("patchExecutor");
                actualSchema?.Properties?.Remove("patchExecutor");
            }

            if (operation.RequestBody.Content.TryGetValue(JsonMergePatchDocument.ContentType, out var content))
            {
                content.Schema = payloadSchemaReference;
            }
        }
    }
}