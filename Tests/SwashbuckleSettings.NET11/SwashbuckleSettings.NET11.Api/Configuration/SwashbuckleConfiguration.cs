using System.Reflection;
using System.Text;
using Asp.Versioning.ApiExplorer;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using SwashbuckleSettings.NET11.Api.Filters;
using SwashbuckleSettings.NET11.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Api.Configuration
{
    public static class SwashbuckleConfiguration
    {
        private const string SwaggerUiVersion = "5.34.1";
        // Swashbuckle's own index page, with the Swagger UI assets taken from swagger-ui-dist instead of the bundled copy.
        private static readonly string SwaggerUiIndex = $$"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>%(DocumentTitle)</title>
    <link rel="stylesheet" type="text/css" href="https://unpkg.com/swagger-ui-dist@{{SwaggerUiVersion}}/swagger-ui.css">
    %(HeadContent)
</head>
<body>
    <div id="swagger-ui"></div>
    <script src="https://unpkg.com/swagger-ui-dist@{{SwaggerUiVersion}}/swagger-ui-bundle.js"></script>
    <script src="https://unpkg.com/swagger-ui-dist@{{SwaggerUiVersion}}/swagger-ui-standalone-preset.js"></script>
    <script>
        window.onload = function () {
            var configObject = JSON.parse('%(ConfigObject)');
            // Swashbuckle's SubmitMethod enum has no QUERY, so "Try it out" would be hidden for QUERY operations.
            configObject.supportedSubmitMethods = (configObject.supportedSubmitMethods || []).concat(["query"]);
            configObject.dom_id = "#swagger-ui";
            configObject.presets = [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset];
            configObject.layout = "StandaloneLayout";
            window.ui = SwaggerUIBundle(configObject);
        };
    </script>
</body>
</html>
""";

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ApiVersionSwaggerGenOptions>();
            services.AddSwaggerGen(
                options =>
                {
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
                    options.OperationFilter<JsonMergePatchOperationFilter>();
                    options.OperationFilter<BinaryContentFilter>();
                    options.OperationFilter<HideRouteParametersFromBodyOperationFilter>();
                    options.SchemaFilter<TypeSchemaFilter>();
                });
            services.Configure<SwaggerUIOptions>(options =>
            {
                options.IndexStream = () => new MemoryStream(Encoding.UTF8.GetBytes(SwaggerUiIndex));
            });
            services.Configure<SwaggerOptions>(options =>
            {
                options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_2;
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
                    options.OAuthAppName("SwashbuckleSettings.NET11 API");
                    options.EnableDeepLinking();
                    options.DisplayOperationId();
                    options.DefaultModelsExpandDepth(2);
                    options.DefaultModelRendering(ModelRendering.Example);
                    options.DocExpansion(DocExpansion.List);
                    options.ShowExtensions();
                    options.EnableFilter(string.Empty);
                    AddSwaggerEndpoints(app, options);
                });
        }

        private static string SchemaIdSelector(Type modelType)
        {
            if (modelType.IsArray)
            {
                var elementType = modelType.GetElementType()!;
                return $"{SchemaIdSelector(elementType)}Array";
            }

            var modelName = modelType.Name.Replace("+", ".");

            if (!modelType.IsConstructedGenericType)
            {
                return modelName;
            }

            var baseName = modelName.Split('`').First();

            var genericArgs = modelType.GetGenericArguments()
                .Select(SchemaIdSelector)
                .ToArray();

            return $"{baseName}Of{string.Join("And", genericArgs)}";
        }

        private static void AddSwaggerEndpoints(IApplicationBuilder app, SwaggerUIOptions options)
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var groupName in provider.ApiVersionDescriptions
                .OrderByDescending(o => o.ApiVersion)
                .Select(a => a.GroupName))
            {
                options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", $"{options.OAuthConfigObject.AppName} {groupName}");
            }
        }
    }

    internal class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema is not OpenApiSchema concreteSchema)
            {
                return;
            }

            if (concreteSchema.Properties == null || concreteSchema.Required == null)
            {
                return;
            }
            var additionalRequiredProps = concreteSchema.Properties
                .Where(x => (x.Value is OpenApiSchema propSchema)
                    && (propSchema.Type & JsonSchemaType.Null) == 0
                    && !concreteSchema.Required.Contains(x.Key))
                .Select(x => x.Key);

            foreach (var propKey in additionalRequiredProps)
            {
                concreteSchema.Required.Add(propKey);
            }
        }
    }
}