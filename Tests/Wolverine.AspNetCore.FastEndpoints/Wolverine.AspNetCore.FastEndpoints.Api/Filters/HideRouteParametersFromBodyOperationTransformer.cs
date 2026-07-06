using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Scalar.HideRouteParametersFromBodyOperationTransformer", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Api.Filters
{
    /// <summary>
    /// Operation transformer that removes properties from request body schema when they are already defined as route parameters.
    /// This prevents duplicate documentation of parameters that are supplied via the URL.
    /// </summary>
    public class HideRouteParametersFromBodyOperationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            // Only process operations with both route parameters and a request body
            if (operation.Parameters == null || operation.RequestBody?.Content == null)
            {
                return Task.CompletedTask;
            }

            // Get all route parameter names (case-insensitive for matching)
            var routeParameters = operation.Parameters
                .Where(p => p.In == ParameterLocation.Path)
                .Select(p => p.Name?.ToLowerInvariant())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToHashSet();

            if (routeParameters.Count == 0)
            {
                return Task.CompletedTask;
            }

            // Process each content type in the request body
            foreach (var contentType in operation.RequestBody.Content.Keys.ToList())
            {
                var content = operation.RequestBody.Content[contentType];
                var schema = content.Schema;

                if (schema == null)
                {
                    continue;
                }

                if (schema.Properties == null || schema.Properties.Count == 0)
                {
                    continue;
                }

                // Find properties that match route parameter names (case-insensitive)
                var propertiesToRemove = schema.Properties
                    .Where(property => routeParameters.Contains(property.Key?.ToLowerInvariant()) && !IsPlainStringSchema(property.Value))
                    .Select(property => property.Key)
                    .ToList();

                if (propertiesToRemove.Count == 0)
                {
                    continue;
                }

                // Resolve the concrete schema - content.Schema may be a direct schema or a reference to a shared component DTO
                var concreteSchema = schema as OpenApiSchema ?? (schema as OpenApiSchemaReference)?.RecursiveTarget;

                if (concreteSchema == null)
                {
                    continue;
                }

                // Clone the schema before mutating - schema may be shared across every operation that references the same DTO
                var clonedSchema = (OpenApiSchema)concreteSchema.CreateShallowCopy();

                if (clonedSchema.Properties == null)
                {
                    continue;
                }

                // Remove matching properties from the clone only
                foreach (var propertyName in propertiesToRemove)
                {
                    if (propertyName != null)
                    {
                        clonedSchema.Properties.Remove(propertyName);
                        clonedSchema.Required?.Remove(propertyName);
                    }
                }

                // Point this operation's content at the clone instead of the shared original
                content.Schema = clonedSchema;
            }

            return Task.CompletedTask;
        }

        private static bool IsPlainStringSchema(IOpenApiSchema schema)
        {
            return schema is OpenApiSchema openApiSchema && (openApiSchema.Type & JsonSchemaType.String) == JsonSchemaType.String && openApiSchema.Format != "uuid";
        }
    }
}