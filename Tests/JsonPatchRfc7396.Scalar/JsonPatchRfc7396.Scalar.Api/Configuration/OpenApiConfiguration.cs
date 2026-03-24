using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Api.Filters;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Api.Configuration
{
    public static class OpenApiConfiguration
    {
        public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
        {
            services.AddOpenApi(
                options =>
                {
                    options.AddSchemaTransformer(
                        (schema, context, cancellationToken) =>
                        {
                            if (context.JsonTypeInfo.Type.IsValueType || context.JsonTypeInfo.Type == typeof(String) || context.JsonTypeInfo.Type == typeof(string))
                            {
                                return Task.CompletedTask;
                            }

                            if (schema.Metadata == null || !schema.Metadata.TryGetValue("x-schema-id", out object? _))
                            {
                                return Task.CompletedTask;
                            }

                            var schemaId = SchemaIdSelector(context.JsonTypeInfo.Type);
                            schema.Metadata["x-schema-id"] = schemaId;
                            schema.Title = schemaId;

                            return Task.CompletedTask;
                        });
                    options.AddOperationTransformer(new JsonMergePatchOperationTransformer());

                    options.AddOperationTransformer(new HideRouteParametersFromBodyOperationTransformer());
                });
            return services;
        }

        private static string SchemaIdSelector(Type modelType)
        {
            if (modelType.IsArray)
            {
                var elementType = modelType.GetElementType()!;
                return $"{SchemaIdSelector(elementType)}Array";
            }

            var typeName = modelType.FullName?.Replace("+", "_") ?? modelType.Name.Replace("+", "_");

            if (!modelType.IsConstructedGenericType)
            {
                return typeName;
            }

            var genericTypeDefName = modelType.GetGenericTypeDefinition().FullName;
            var baseName = (genericTypeDefName?.Split('`')[0] ?? modelType.Name.Split('`')[0]).Replace("+", "_");

            var genericArgs = modelType.GetGenericArguments()
                .Select(SchemaIdSelector)
                .ToArray();

            return $"{baseName}_Of_{string.Join("_And_", genericArgs)}";
        }
    }
}