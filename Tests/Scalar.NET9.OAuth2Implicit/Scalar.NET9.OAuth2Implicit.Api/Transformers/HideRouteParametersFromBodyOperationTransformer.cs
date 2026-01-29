using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Scalar.HideRouteParametersFromBodyOperationTransformer", Version = "1.0")]

namespace Scalar.NET9.OAuth2Implicit.Api.Transformers
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

                // Note: Schema references are typically already resolved at this stage
                if (schema.Reference != null)
                {
                    // Schema is a reference, properties may not be directly accessible
                    continue;
                }

                if (schema.Properties == null || schema.Properties.Count == 0)
                {
                    continue;
                }

                // Find properties that match route parameter names (case-insensitive)
                var propertiesToRemove = schema.Properties.Keys
                    .Where(key => routeParameters.Contains(key?.ToLowerInvariant()))
                    .ToList();

                if (propertiesToRemove.Count == 0)
                {
                    continue;
                }

                // Create a new schema with the filtered properties
                var newSchema = new OpenApiSchema(schema);

                // Remove matching properties from the new schema
                foreach (var propertyName in propertiesToRemove)
                {
                    newSchema.Properties.Remove(propertyName);
                    newSchema.Required?.Remove(propertyName);
                }

                // Replace the content schema with the new filtered schema
                operation.RequestBody.Content[contentType].Schema = newSchema;
            }

            return Task.CompletedTask;
        }
    }
}