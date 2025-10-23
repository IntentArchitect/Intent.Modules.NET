using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Finbuckle.SharedDatabase.TestApplication.Api.Filters;
using Finbuckle.SharedDatabase.TestApplication.Application;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Api.Configuration
{
    public static class SwashbuckleConfiguration
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "Finbuckle.SharedDatabase.TestApplication API"
                        });
                    options.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
                    options.SupportNonNullableReferenceTypes();
                    options.CustomSchemaIds(SchemaIdSelector);

                    var apiXmlFile = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                    if (File.Exists(apiXmlFile))
                    {
                        options.IncludeXmlComments(apiXmlFile);
                    }

                    var applicationXmlFile = Path.Combine(AppContext.BaseDirectory, $"{typeof(DependencyInjection).Assembly.GetName().Name}.xml");
                    if (File.Exists(applicationXmlFile))
                    {
                        options.IncludeXmlComments(applicationXmlFile);
                    }
                    options.OperationFilter<TenantHeaderOperationFilter>();
                    options.SchemaFilter<TypeSchemaFilter>();
                });
            return services;
        }

        public static void UseSwashbuckle(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Finbuckle.SharedDatabase.TestApplication API V1");
                    options.OAuthAppName("Finbuckle.SharedDatabase.TestApplication API");
                    options.EnableDeepLinking();
                    options.DisplayOperationId();
                    options.DefaultModelsExpandDepth(2);
                    options.DefaultModelRendering(ModelRendering.Example);
                    options.DocExpansion(DocExpansion.List);
                    options.ShowExtensions();
                    options.EnableFilter(string.Empty);
                });
        }

        private static string SchemaIdSelector(Type modelType)
        {
            if (modelType.IsArray)
            {
                var elementType = modelType.GetElementType()!;
                return $"{SchemaIdSelector(elementType)}Array";
            }

            var typeName = modelType.FullName?.Replace("+", ".") ?? modelType.Name.Replace("+", ".");

            if (!modelType.IsConstructedGenericType)
            {
                return typeName;
            }

            var genericTypeDefName = modelType.GetGenericTypeDefinition().FullName;
            var baseName = (genericTypeDefName?.Split('`')[0] ?? modelType.Name.Split('`')[0]).Replace("+", ".");

            var genericArgs = modelType.GetGenericArguments()
                .Select(SchemaIdSelector)
                .ToArray();

            return $"{baseName}_Of_{string.Join("_And_", genericArgs)}";
        }
    }

    internal class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var additionalRequiredProps = schema.Properties
                .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
                .Select(x => x.Key);

            foreach (var propKey in additionalRequiredProps)
            {
                schema.Required.Add(propKey);
            }
        }
    }
}