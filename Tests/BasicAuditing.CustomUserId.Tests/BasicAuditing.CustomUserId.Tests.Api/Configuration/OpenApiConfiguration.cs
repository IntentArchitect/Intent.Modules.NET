using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Api.Configuration
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
                            string? transformedTypeName = context.JsonTypeInfo.Type.FullName?.Replace("+", ".", StringComparison.Ordinal);
                            schema.Annotations["x-schema-id"] = transformedTypeName;
                            schema.Title = transformedTypeName;
                            return Task.CompletedTask;
                        });
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        document.Components ??= new();

                        document.Components.SecuritySchemes["Bearer"] = new()
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT"
                        };

                        var bearerSchemeReference = new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        };

                        var securityStatement = new OpenApiSecurityRequirement
                        {
                            [bearerSchemeReference] = new List<string>()
                        };

                        document.SecurityRequirements.Add(securityStatement);
                        return Task.CompletedTask;
                    });
                });
            return services;
        }
    }
}