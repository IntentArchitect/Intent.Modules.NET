using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Scalar.NET10.Bearer.Api.Filters;
using Scalar.NET10.Bearer.Api.Transformers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration", Version = "1.0")]

namespace Scalar.NET10.Bearer.Api.Configuration
{
    public static class OpenApiConfiguration
    {
        private const string OpenApiVersion = "3.2.0";
        private static readonly Regex OpenApiDocumentPath = new(@"^/openapi/([^/]+)\.json$");

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
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        document.Components ??= new OpenApiComponents();
                        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                        document.Components.SecuritySchemes.Add("bearer", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            In = ParameterLocation.Header,
                            BearerFormat = "Json Web Token"
                        });

                        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations ?? []))
                        {
                            operation.Value.Security ??= [];
                            operation.Value.Security.Add(new OpenApiSecurityRequirement
                            {
                                { new OpenApiSecuritySchemeReference("Bearer", document), [] }
                            });
                        }

                        return Task.CompletedTask;
                    });

                    options.AddDocumentTransformer<HttpQueryDocumentTransformer>();

                    options.AddOperationTransformer(new HideRouteParametersFromBodyOperationTransformer());
                });
            return services;
        }

        /// <summary>
        /// Serves /openapi/{documentName}.json as OpenAPI 3.2. ASP.NET Core 10 writes at most 3.1 and has no serializer hook,
        /// so this writes the same document as 3.1 (3.2 only adds to it) and labels it 3.2.0, the first version with a native
        /// "query" operation. Registered ahead of MapOpenApi, which it answers for. Drop it once ASP.NET Core writes 3.2 itself.
        /// </summary>
        public static IApplicationBuilder UseHttpQueryOpenApi(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var match = OpenApiDocumentPath.Match(context.Request.Path.Value ?? string.Empty);
                var provider = match.Success && HttpMethods.IsGet(context.Request.Method) ? context.RequestServices.GetKeyedService<IOpenApiDocumentProvider>(match.Groups[1].Value) : null;

                if (provider is null)
                {
                    await next(context);
                    return;
                }

                var document = await provider.GetOpenApiDocumentAsync(context.RequestAborted);
                using var buffer = new StringWriter();
                document.SerializeAsV31(new OpenApiJsonWriter(buffer));
                var json = JsonNode.Parse(buffer.ToString())!.AsObject();
                json["openapi"] = OpenApiVersion;

                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(json.ToJsonString(), context.RequestAborted);
            });
            return app;
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