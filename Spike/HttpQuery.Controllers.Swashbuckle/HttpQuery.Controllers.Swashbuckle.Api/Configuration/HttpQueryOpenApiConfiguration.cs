using System.Text;
using HttpQuery.Controllers.Swashbuckle.Api.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace HttpQuery.Controllers.Swashbuckle.Api.Configuration
{
    /// <summary>
    /// Everything Swashbuckle needs to describe HTTP QUERY endpoints, registered with one call from ConfigureSwagger.
    /// </summary>
    public static class HttpQueryOpenApiConfiguration
    {
        // Swagger UI shows QUERY operations from 5.34.0; Swashbuckle 10.3.1 still bundles 5.33.3.
        public const string SwaggerUiVersion = "5.34.1";

        public static IServiceCollection ConfigureHttpQueryOpenApi(this IServiceCollection services)
        {
            services.Configure<SwaggerGenOptions>(options =>
            {
                options.DocInclusionPredicate(HttpQueryDocumentFilter.ExcludeFromMainPass);
                options.DocumentFilter<HttpQueryDocumentFilter>();
            });
            services.Configure<SwaggerOptions>(options =>
            {
                options.CustomDocumentSerializer = new OpenApi32DocumentSerializer();
            });
            services.Configure<SwaggerUIOptions>(options =>
            {
                options.IndexStream = () => new MemoryStream(Encoding.UTF8.GetBytes(SwaggerUiIndex));
            });
            return services;
        }

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
    }
}
