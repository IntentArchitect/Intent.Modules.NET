using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Morcatko.AspNetCore.JsonMergePatch;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchOperationTransformer", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Api.Filters
{
    public class JsonMergePatchOperationTransformer : IOpenApiOperationTransformer
    {
        public async Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            if (context.Description.HttpMethod != "PATCH")
            {
                return;
            }

            if (operation.RequestBody?.Content == null)
            {
                return;
            }

            var patchParam = context.Description.ParameterDescriptions
                .FirstOrDefault(param => param.Type.IsGenericType && param.Type.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>));

            if (patchParam == null)
            {
                return;
            }

            var payloadType = patchParam.Type.GetGenericArguments()[0];

            var payloadSchemaReference = await context.GetOrCreateSchemaAsync(payloadType, cancellationToken: cancellationToken);
            if (payloadSchemaReference == null)
            {
                return;
            }

            payloadSchemaReference.Required?.Remove("patchExecutor");
            payloadSchemaReference.Properties?.Remove("patchExecutor");

            if (operation.RequestBody.Content.TryGetValue(JsonMergePatchDocument.ContentType, out var content))
            {
                content.Schema = payloadSchemaReference;
            }
        }
    }
}