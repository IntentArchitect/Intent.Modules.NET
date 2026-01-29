using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Models;
using Scalar.NET9.OAuth2Implicit.Api.Transformers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration", Version = "1.0")]

namespace Scalar.NET9.OAuth2Implicit.Api.Configuration
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

                            if (schema.Annotations == null || !schema.Annotations.TryGetValue("x-schema-id", out object? _))
                            {
                                return Task.CompletedTask;
                            }

                            var schemaId = SchemaIdSelector(context.JsonTypeInfo.Type);
                            schema.Annotations["x-schema-id"] = schemaId;
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

                        document.Components ??= new();
                        document.Components.SecuritySchemes["OidcImplicit"] = new()
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri(authorizationUrl),
                                    Scopes = scopes.ToDictionary(s => s, s => $"Access to {s}")
                                }
                            }
                        };

                        var oidcSchemeReference = new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "OidcImplicit",
                                Type = ReferenceType.SecurityScheme
                            }
                        };

                        var securityStatement = new OpenApiSecurityRequirement
                        {
                            [oidcSchemeReference] = scopes.ToList()
                        };

                        document.SecurityRequirements.Add(securityStatement);
                        return Task.CompletedTask;
                    });

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