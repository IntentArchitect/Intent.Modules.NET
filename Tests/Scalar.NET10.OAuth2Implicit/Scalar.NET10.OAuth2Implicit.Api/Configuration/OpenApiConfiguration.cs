using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration", Version = "1.0")]

namespace Scalar.NET10.OAuth2Implicit.Api.Configuration
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
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        var configuration = context.ApplicationServices.GetRequiredService<IConfiguration>();
                        var oidcSection = configuration.GetSection("OpenApi:Oidc");
                        var authorizationUrl = oidcSection.GetValue<string>("AuthorizationUrl");
                        var scopes = oidcSection.GetSection("Scopes").Get<string[]>() ?? Array.Empty<string>();

                        if (string.IsNullOrEmpty(authorizationUrl))
                        {
                            throw new ArgumentException("You have not configured your AuthorizationUrl", nameof(authorizationUrl));
                        }

                        document.Components ??= new OpenApiComponents();
                        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                        document.Components.SecuritySchemes.Add("oauth2", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                AuthorizationCode = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri(authorizationUrl),
                                    Scopes = scopes.ToDictionary(s => s, s => $"Access to {s}")
                                }
                            }
                        });

                        document.Security ??= new List<OpenApiSecurityRequirement>();
                        document.Security.Add(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecuritySchemeReference("oauth2"),
                                scopes.ToList()
                            }
                        });

                        document.SetReferenceHostDocument();

                        return Task.CompletedTask;
                    });
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